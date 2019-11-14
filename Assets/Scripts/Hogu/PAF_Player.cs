using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random;

public class PAF_Player : MonoBehaviour
{
    #region Events
    public static event Action<bool, int> OnFall = null;
    #endregion

    #region Fields
    public static List<PAF_Player> Players = new List<PAF_Player>(); 

    [SerializeField] bool isPlayerOne = true;
    public bool IsPlayerOne { get { return isPlayerOne; } }
    [SerializeField] bool stunned = false;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isInvulnerable = false;
    bool idle = false;
    bool falling = false;
    [SerializeField, Range(0, 5)] float attackDelay = .25f;
    [SerializeField, Range(0, 50)] float attackForce = 25;
    [SerializeField, Range(0, 5)] float invulnerableTime = .5f;
    [SerializeField, Range(0, 5)] float stunTime = .5f;
    [SerializeField, Range(0, 5)] float fallTime = .5f;
    [SerializeField, Range(.1f, 1)] float fallDetectionSize = .5f;
    [SerializeField, Range(1.0f, 5.0f)] float recoilDistance = 1; 

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

    private void Awake()
    {
        Players.Add(this);
    }
    private void Start()
    {
        PAF_GameManager.OnGameEnd += EndGame;
        PAF_GameManager.OnPlayerScored += CheckScore;
        playerAnimator.Init(playerSpeed);
    }

    private void OnDestroy()
    {
        Players.Remove(this); 
        PAF_GameManager.OnGameEnd -= EndGame;
        PAF_GameManager.OnPlayerScored -= CheckScore;
    }

    private void Update()
    {
        if(PAF_GameManager.Instance && !PAF_GameManager.Instance.GameIsReadyToStart)
        {
            if (PAF_GameManager.Instance.PlayersAreReadyToStart) return;

            #if UNITY_EDITOR
            // * DEBUG POUR UNE SEULE MANETTE
            if(Input.GetKeyDown(KeyCode.A))
                PAF_GameManager.Instance?.SetPlayerReady(isPlayerOne);
            #endif

            if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2) ||
                Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3))
                PAF_GameManager.Instance?.SetPlayerReady(isPlayerOne);
            return; 
        }
        Move();
        if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0) ||
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1) ||
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2) ||
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3) &&
            canAttack && !stunned) playerAnimator.SetAttack();
    }

    void Move()
    {
        if (!IsReady || falling) return;
        if (!stunned)
        {
            Vector3 _dir = new Vector3(Input.GetAxis(isPlayerOne ? "Horizontal1" : "Horizontal2"), 0, Input.GetAxis(isPlayerOne ? "Vertical1" : "Vertical2"));
            if (idle = _dir.magnitude < .1f)
            {
                playerAnimator.SetMoving(idle);
                accelerationTimer = 0; 
                return;
            }
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
        else idle = true;
        playerAnimator.SetMoving(idle);
    }

    private void Fall()
    {
        falling = true;
        AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetFallPlayer();
        if (_clip) audioPlayer.PlayOneShot(_clip, .8f);
        playerAnimator.SetFalling();
        playerAnimator.SetMoving(false);
        OnFall?.Invoke(isPlayerOne, fallScoreIncrease);
    }

    public void Interact()
    {
        if (!canAttack || !IsReady) return; 
        List<PAF_Fruit> _fruitsHit = new List<PAF_Fruit>();
        Collider[] _hitItems = Physics.OverlapSphere(transform.position, sightRange, interactLayer);
        foreach (Collider _hit in _hitItems)
        {
            if (Vector3.Angle(transform.forward, (_hit.transform.position - transform.position).normalized) > (fieldOfView / 2) && Vector3.Distance(_hit.transform.position, transform.position) > closeRange)
                continue;
            PAF_Fruit _item = _hit.transform.GetComponent<PAF_Fruit>();
            if (_item)
            {
                if (_fruitsHit.Contains(_item)) continue;
                _fruitsHit.Add(_item);
                _item.AddForce(transform.forward * attackForce, this);
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetHitFruit();
                if (_clip) audioPlayer.PlayOneShot(_clip, .9f);
                InstantiateHitFX(_hit);
            }
            PAF_Bulb _bulb = _hit.transform.GetComponent<PAF_Bulb>();
            if(_bulb)
            {
                _bulb.Hit(this);
                InstantiateHitFX(_hit);
            }
            PAF_Player _player = _hit.transform.GetComponent<PAF_Player>();
            if (_player && _player != this)
            {
                _player.Stun(transform.position);
                InstantiateHitFX(_hit);
            }
            if(_hit.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetHitWall();
                if (_clip) audioPlayer.PlayOneShot(_clip);
            }
        }
        AudioClip _clipSwipe = PAF_GameManager.Instance?.SoundDatas.GetHitNone();
        if (_clipSwipe) audioPlayer.PlayOneShot(_clipSwipe);
        
        StartCoroutine(InvertBoolDelay((state) => { canAttack = state; }, attackDelay));
    }

    void InstantiateHitFX(Collider _collider)
    {
        ParticleSystem _system = PAF_GameManager.Instance?.VFXDatas?.HitFX;
        if (_system) Instantiate(_system.gameObject, _collider.ClosestPointOnBounds(transform.position), Quaternion.identity);
    }

    public void Stun(Vector3 _from)
    {
        if (!IsReady || isInvulnerable) return;
        isInvulnerable = true;
        transform.rotation = Quaternion.LookRotation(_from - transform.position);
        StartCoroutine(StunFlashTimer());
        InvokeRepeating("Flash", 0, .1f);
        AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetHitPlayer();
        if (_clip) audioPlayer.PlayOneShot(_clip);
        playerAnimator.SetStunned();

        // Confused FX
        GameObject _system = PAF_GameManager.Instance?.VFXDatas?.ConfusedFX;
        if (_system) Instantiate(_system, new Vector3(transform.position.x - (transform.forward.x * .75f), transform.position.y + 1, transform.position.z - (transform.forward.z * .75f)), Quaternion.identity);
    }

    IEnumerator FallInvulnerable()
    {
        isInvulnerable = true;
        InvokeRepeating("Flash", 0, .1f);
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
        CancelInvoke("Flash");
        if (playerRenderer) playerRenderer.enabled = true;
        if (stickRend) stickRend.enabled = true;
    }

    void Flash()
    {
        playerRenderer.enabled = !playerRenderer.enabled;
        if (stickRend) stickRend.enabled = !stickRend.enabled;
    }

    IEnumerator StunFlashTimer()
    {
        stunned = true;
        StartCoroutine(InvertBoolDelay((state) => { isInvulnerable = !state; }, invulnerableTime + stunTime));
        yield return new WaitForSeconds(stunTime );
         stunned = false;
        yield return new WaitForSeconds(invulnerableTime);
        CancelInvoke("Flash");
        if (playerRenderer) playerRenderer.enabled = true;
        if (stickRend) stickRend.enabled = true;
    }

    private IEnumerator ApplyRecoil(Vector3 _from)
    {
        Vector3 _basePosition = transform.position;
        Vector3 _endPosition = transform.position + Vector3.ClampMagnitude((transform.position - _from), recoilDistance);
        float _timer = 0;
        float _totalTimer = stunTime + invulnerableTime;
        float _delta = 0; 
        while (stunned)
        {
            transform.position = Vector3.Lerp(_basePosition, _endPosition, _delta); 
            yield return new WaitForEndOfFrame();
            _timer += Time.deltaTime;
            _delta = (_timer / _totalTimer); 
        }
    }

    public static IEnumerator InvertBoolDelay(System.Action<bool> _callBack, float _time)
    {
        bool _state = false;
        _callBack(_state);
        yield return new WaitForSeconds(_time);
        _state = true;
        _callBack(_state);
    }

    public void Respawn()
    {
        if (!falling) return;
        StartCoroutine(FallInvulnerable());
        transform.GetChild(0).localScale = Vector3.one;

        // Spash FX
        ParticleSystem _system = PAF_GameManager.Instance?.VFXDatas?.SplashFX;
        if (_system) Instantiate(_system.gameObject, new Vector3(transform.position.x, transform.position.y - 5, transform.position.z + 5), Quaternion.identity);

        if (PAF_DalleManager.I.AllRespawnableDalles.Count <= 0)
        {
            transform.position = Vector3.zero;
            falling = false;
            return;
        }
        MeshCollider _dalle = PAF_DalleManager.I.AllRespawnableDalles[Random.Range(0, PAF_DalleManager.I.AllRespawnableDalles.Count)].GetComponent<MeshCollider>();
        Vector3 _spawnPos = new Vector3(Random.Range(_dalle.bounds.min.x, _dalle.bounds.max.x), 0, Random.Range(_dalle.bounds.min.z, _dalle.bounds.max.z));
        transform.position = _spawnPos;
        falling = false;
    }

    public void InstantiateRunFX()
    {
        // Run FX
        ParticleSystem _system = PAF_GameManager.Instance?.VFXDatas?.StepSmokeFX;
        if (_system) Instantiate(_system.gameObject, transform.position, Quaternion.identity);
    }

    public void EndGame(int _one, int _two)
    {
        playerAnimator.SetMoving(true); 
    }

    public void Recoil(Vector3 _from)
    {
        Stun(_from);
        StartCoroutine(ApplyRecoil(_from)); 
    }

    private void CheckScore(bool _isPlayerOne, int _score)
    {
        if (!m_leaderVFX) return; 
        if(_isPlayerOne == isPlayerOne)
        {
            if(_isPlayerOne)
            {
                m_leaderVFX.SetActive(PAF_GameManager.Instance.PlayerOneIsLeading);
            }
            else
            {
                m_leaderVFX.SetActive(!PAF_GameManager.Instance.PlayerOneIsLeading);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + (transform.forward * .5f), .25f);
        Gizmos.DrawSphere(transform.position + ((transform.forward * .5f) + transform.right * .5f), .25f);
        Gizmos.DrawSphere(transform.position + ((transform.forward * .5f) - transform.right * .5f), .25f);
    }
}
