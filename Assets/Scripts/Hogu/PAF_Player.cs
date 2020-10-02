using UnityEngine;

using Random = UnityEngine.Random;

public class PAF_Player : MonoBehaviour
{
    #region Fields
    public static readonly PAF_Player[] Players = new PAF_Player[2];

    [SerializeField] bool isPlayerOne = true;
    public bool IsPlayerOne => isPlayerOne;

    [SerializeField] bool isStunned = false;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isInvulnerable = false;
    bool idle = false;
    bool isFalling = false;
    [SerializeField, Range(0, 5)] float attackDelay = .25f;
    [SerializeField, Range(0, 50)] float attackForce = 25;
    [SerializeField, Range(0, 5)] float invulnerableTime = .5f;
    [SerializeField, Range(0, 5)] float stunTime = .5f;
    [SerializeField, Range(0, 5)] float fallTime = .5f;
    [SerializeField, Range(.1f, 1)] float fallDetectionSize = .5f;
    [SerializeField, Range(1.0f, 5.0f)] float recoilDistance = 1;
    private float resetMoveTimer = 0;

    private const float colliderRadius = .5f;
    private const int fallScoreIncrease = -10;

    [SerializeField] private AnimationCurve accelerationCurve = null;
    private float accelerationTimer = 0;
    [SerializeField, Range(0, 10)] float playerSpeed = 5;

    [Header("Sight")]
    [SerializeField, Range(0, 10)] float sightRange = 1.5f;
    [SerializeField, Range(0, 360)] int fieldOfView = 60;
    [SerializeField, Range(0, 10.0f)] float closeRange = 1.0f;
    #endregion

    #region Sounds
    [Header("Sounds")]
    [SerializeField] AudioSource audioPlayer = null;
    #endregion

    #region Objects
    [SerializeField] Collider moveArea = null;
    [SerializeField] Renderer playerRenderer = null;
    [SerializeField] Renderer stickRend = null;
    [SerializeField] PAF_PlayerAnimator playerAnimator = null;
    #endregion

    public bool IsReady => moveArea && playerRenderer && playerAnimator && audioPlayer;

    #region Layers
    [Space, Header("Layers")]
    [SerializeField] LayerMask groundLayer = 0;
    [SerializeField] LayerMask obstacleLayer = 0;
    [SerializeField] LayerMask interactLayer = 0;

    [Header("Leader VFX")]
    [SerializeField] private GameObject m_leaderVFX = null;
    #endregion

    private void Awake() => Players[isPlayerOne ? 0 : 1] = this;

    private void Start()
    {
        playerAnimator.Init(playerSpeed);
        controllerBounds = PAF_UIManager.Instance.GetPlayerBounds(isPlayerOne);
    }

    private void OnDestroy() => Players[isPlayerOne ? 0 : 1] = null;

    private Vector3 recoilStart = new Vector3();
    private Vector3 recoilEnd = new Vector3();

    private bool doRecoil = false;
    [SerializeField] private float recoilTime = 0;
    private float recoilVar = 0;

    private float invulnerableVar = 0;

    private bool isFlashing = false;
    private float flashVar = 0;
    private int flashAmount = 0;

    private float stunVar = 0;
    private float canAttackVar = 0;

    private bool doRespawn = false;
    private float respawnVar = 0;

    private RectTransform controllerBounds = null;
    private Touch touch = new Touch();

    private bool wasTouch = false;
    private float touchDuration = 0;

    private void Update()
    {
        // Respawn.
        if (doRespawn)
        {
            respawnVar += Time.deltaTime;
            if (respawnVar >= .5f)
            {
                doRespawn = false;
                Respawn();
            }
        }

        // Invulnerability.
        if (isInvulnerable)
        {
            invulnerableVar += Time.deltaTime;
            if (invulnerableVar >= invulnerableTime)
                isInvulnerable = false;
        }

        // Flash.
        if (isFlashing)
        {
            flashVar += Time.deltaTime;
            if (flashVar >= .1f)
            {
                flashVar = 0;
                flashAmount--;

                if (flashAmount <= 0)
                {
                    isFlashing = false;
                    playerRenderer.enabled = true;
                    stickRend.enabled = true;
                }
                else
                {
                    playerRenderer.enabled = !playerRenderer.enabled;
                    stickRend.enabled = !stickRend.enabled;
                }
            }
        }

        // Stun.
        if (isStunned)
        {
            stunVar += Time.deltaTime;
            if (stunVar >= stunTime)
                isStunned = false;
        }

        // Recoil.
        if (doRecoil)
        {
            recoilVar += Time.deltaTime;
            if (recoilVar >= recoilTime)
            {
                doRecoil = false;
                recoilVar = recoilTime;
            }

            transform.position = Vector3.Lerp(recoilStart, recoilEnd, recoilVar / recoilTime);
        }

        // Attack cooldown.
        if (!canAttack)
        {
            canAttackVar += Time.deltaTime;
            if (canAttackVar >= attackDelay)
                canAttack = true;
        }

        if (!PAF_GameManager.Instance.GameIsReadyToStart)
        {
            if (PAF_GameManager.Instance.PlayersAreReadyToStart)
                return;

            if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3))
                PAF_GameManager.Instance.SetPlayerReady(isPlayerOne);
        }
        else
        {
            bool _isTouch = false;
            
            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(controllerBounds, Input.mousePosition, PAF_GameManager.Instance.Camera))
                {
                    _isTouch = true;
                    touch.position = Input.mousePosition;
                }
            }
            #else
            int _amount = Input.touchCount;
            for (int _i = 0; _i < _amount; _i++)
            {
                Touch _touch = Input.GetTouch(_i);
                if (RectTransformUtility.RectangleContainsScreenPoint(controllerBounds, _touch.position, PAF_GameManager.Instance.Camera))
                {
                    _isTouch = true;
                    touch = _touch;
                    break;
                }
            }
            #endif

            if (IsReady && !isFalling)
                Move(_isTouch);

            if (canAttack && !isStunned)
            {
                if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3))
                    playerAnimator.SetAttack();

                if (wasTouch && !_isTouch && touchDuration < .1f)
                    playerAnimator.SetAttack();
            }

            wasTouch = _isTouch;
            if (_isTouch)
                touchDuration += Time.deltaTime;
            else
                touchDuration = 0;
    }
    }

    void Move(bool _isTouch)
    {
        if (!isStunned)
        {
            if (!_isTouch || (touchDuration < .1f) || !RectTransformUtility.ScreenPointToLocalPointInRectangle(controllerBounds, touch.position, PAF_GameManager.Instance.Camera, out Vector2 _local))
            {
                idle = true;
                resetMoveTimer += Time.deltaTime;

                PAF_UIManager.Instance.UpdatePlayerJoystick(isPlayerOne, Vector2.zero);

                if (resetMoveTimer > .1f)
                {
                    playerAnimator.SetMoving(idle);
                    accelerationTimer = 0;
                    resetMoveTimer = 0;
                }
                return;
            }
            else if (resetMoveTimer > 0)
                resetMoveTimer = 0;

            PAF_UIManager.Instance.UpdatePlayerJoystick(isPlayerOne, _local);

            idle = false;
            Vector3 _dir = new Vector3(_local.x, 0, _local.y).normalized;
            transform.rotation = Quaternion.LookRotation(_dir);
            accelerationTimer += Time.deltaTime;
            accelerationTimer = Mathf.Clamp(accelerationTimer, 0, 1);
            Vector3 _movement = _dir * Time.deltaTime * playerSpeed * 3 * accelerationCurve.Evaluate(accelerationTimer);

            RaycastHit _hit;
            Vector3[] _from = new Vector3[] { transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + (transform.forward.z * colliderRadius)), new Vector3(transform.position.x, transform.position.y, transform.position.z - (transform.forward.z * colliderRadius)) };
            for (int _i = 0; _i < 3; _i++)
            {
                if (Physics.Raycast(_from[_i], new Vector3(transform.forward.x, 0, 0), out _hit, Mathf.Abs(_movement.x) + colliderRadius, obstacleLayer))
                {
                    _movement.x = (_hit.distance - colliderRadius) * Mathf.Sign(_movement.x);
                }
            }
            _from = new Vector3[] { transform.position, new Vector3(transform.position.x + (transform.forward.x * colliderRadius), transform.position.y, transform.position.z), new Vector3(transform.position.x - (transform.forward.z * colliderRadius), transform.position.y, transform.position.z) };
            for (int _i = 0; _i < 3; _i++)
            {
                if (Physics.Raycast(_from[_i], new Vector3(0, 0, transform.forward.z), out _hit, Mathf.Abs(_movement.z) + colliderRadius, obstacleLayer))
                {
                    _movement.z = (_hit.distance - colliderRadius) * Mathf.Sign(_movement.z);
                }
            }
            transform.position += _movement;
            if (!Physics.Raycast(transform.position + (transform.forward + transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer) &&
                !Physics.Raycast(transform.position + (transform.forward - transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer) &&
                !Physics.Raycast(transform.position - (transform.forward + transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer) &&
                !Physics.Raycast(transform.position - (transform.forward - transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer))
            {
                Fall();
                return;
            }
        }
        else
            idle = true;

        playerAnimator.SetMoving(idle);
    }

    private static readonly Collider[] overlapBuffer = new Collider[6];

    public void Interact()
    {
        if (!canAttack || !IsReady)
            return;

        int _amount = Physics.OverlapSphereNonAlloc(transform.position, sightRange, overlapBuffer, interactLayer);
        for (int _i = 0; _i < _amount; _i++)
        {
            Collider _collider = overlapBuffer[_i];

            if (Vector3.Angle(transform.forward, (_collider.transform.position - transform.position).normalized) > (fieldOfView / 2) && Vector3.Distance(_collider.transform.position, transform.position) > closeRange)
                continue;

            if (_collider.TryGetComponent(out PAF_Fruit _fruit))
            {
                _fruit.AddForce(transform.forward * attackForce, this);

                InstantiateHitFX(_collider);
                audioPlayer.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetHitFruit(), .9f);
            }
            else if (_collider.TryGetComponent(out PAF_Bulb _bulb))
            {
                _bulb.Hit(this);
                InstantiateHitFX(_collider);
            }
            else if (_collider.TryGetComponent(out PAF_Player _player) && (_player != this))
            {
                _player.Stun(transform.position);
                InstantiateHitFX(_collider);
            }
            else if (_collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                audioPlayer.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetHitWall());
            }
        }

        audioPlayer.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetHitNone());

        canAttack = false;
        canAttackVar = 0;
    }

    void InstantiateHitFX(Collider _collider)
    {
        ParticleSystem _system = PAF_GameManager.Instance.VFXDatas.HitFX;
        Instantiate(_system.gameObject, _collider.ClosestPointOnBounds(transform.position), Quaternion.identity);
    }

    private void Fall()
    {
        isFalling = true;
        audioPlayer.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetFallPlayer(), .8f);

        playerAnimator.SetFalling();
        playerAnimator.SetMoving(false);

        PAF_GameManager.Instance.IncreasePlayerScore(isPlayerOne, fallScoreIncrease);
    }

    public void DoRespawn()
    {
        doRespawn = true;
        respawnVar = 0;
    }

    private void Respawn()
    {
        if (!isFalling)
            return;

        isInvulnerable = true;
        invulnerableVar = 0;

        isFlashing = true;
        flashAmount = (int)(invulnerableTime / .1f);
        flashVar = 0;

        transform.GetChild(0).localScale = Vector3.one;

        // Spash FX
        GameObject _system = PAF_GameManager.Instance.VFXDatas.SplashFX.gameObject;
        Instantiate(_system, new Vector3(transform.position.x, transform.position.y - 5, transform.position.z + 5), Quaternion.identity);

        if (PAF_DalleManager.I.AllRespawnableDalles.Length <= 0)
        {
            transform.position = Vector3.zero;
        }
        else
        {
            Bounds _dalle = PAF_DalleManager.I.AllRespawnableDalles[Random.Range(0,
                        PAF_DalleManager.I.AllRespawnableDalles.Length)].GetComponent<MeshCollider>().bounds;

            Vector3 _spawnPos = new Vector3(Random.Range(_dalle.min.x, _dalle.max.x), 0, Random.Range(_dalle.min.z, _dalle.max.z));
            transform.position = _spawnPos;
        }

        isFalling = false;
    }

    public void Recoil(Vector3 _from)
    {
        Stun(_from);

        doRecoil = true;
        recoilVar = 0;

        recoilStart = transform.position;
        recoilEnd = recoilStart + ((transform.position - _from).normalized * recoilDistance);
    }

    public void Stun(Vector3 _from)
    {
        if (IsReady && !isInvulnerable)
        {
            isInvulnerable = true;
            transform.rotation = Quaternion.LookRotation(_from - transform.position);

            isStunned = true;
            stunVar = 0;

            isFlashing = true;
            flashAmount = (int)((invulnerableTime + stunTime) / .1f);
            flashVar = 0;

            audioPlayer.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetHitPlayer());
            playerAnimator.SetStunned();

            // Confused FX
            GameObject _system = PAF_GameManager.Instance.VFXDatas.ConfusedFX;
            Instantiate(_system, new Vector3(transform.position.x - (transform.forward.x * .75f), transform.position.y + 1, transform.position.z - (transform.forward.z * .75f)), Quaternion.identity, transform);
        }
    }

    public void EndGame() => playerAnimator.SetMoving(true);

    public void CheckScore(bool _isPlayerOne, int _score)
    {
        if (_isPlayerOne == isPlayerOne)
        {
            bool _value = PAF_GameManager.Instance.PlayerOneIsLeading;
            if (!_isPlayerOne)
                _value = !_value;

            m_leaderVFX.SetActive(_value);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + (transform.forward * .5f), .25f);
        Gizmos.DrawSphere(transform.position + ((transform.forward * .5f) + transform.right * .5f), .25f);
        Gizmos.DrawSphere(transform.position + ((transform.forward * .5f) - transform.right * .5f), .25f);
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(0, 0, 100, 100), (1f / Time.deltaTime).ToString());
    }
}
