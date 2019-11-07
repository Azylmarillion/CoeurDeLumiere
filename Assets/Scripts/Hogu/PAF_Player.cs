using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Player : MonoBehaviour
{
    #region Fields
    [SerializeField] bool isPlayerOne = true;
    public bool IsPlayerOne { get { return isPlayerOne; } }
    [SerializeField] bool stunned = false;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isInvulnerable = false;
    bool idle = false;
    bool falling = false;
    [SerializeField, Range(0, 5)] float attackDelay = .25f;
    [SerializeField, Range(0, 2)] float attackForce = .5f;
    [SerializeField, Range(0, 5)] float invulnerableTime = .5f;
    [SerializeField, Range(0, 5)] float stunTime = .5f;
    [SerializeField, Range(0, 5)] float fallTime = .5f;
    [SerializeField, Range(.1f, 1)] float fallDetectionSize = .5f;

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
    [SerializeField] PAF_PlayerAnimator playerAnimator = null;
    #endregion

    public bool IsReady => moveArea && playerRenderer && playerAnimator && audioPlayer;

    #region Layers
    [Space, Header("Layers")]
    [SerializeField] LayerMask groundLayer = 0;
    [SerializeField] LayerMask obstacleLayer = 0;
    [SerializeField] LayerMask interactLayer = 0;
    #endregion

    private void Start()
    {
        playerAnimator.Init(playerSpeed);
        //InvokeRepeating("StepSounds", 1, .5f); // Gaffe à  ça! Autant le mettre dans l'anim! 
    }

    private void Update()
    {
        if(PAF_GameManager.Instance && !PAF_GameManager.Instance.GameIsReadyToStart)
        {
            
            // * DEBUG POUR UNE SEULE MANETTE
            if(Input.GetKeyDown(KeyCode.A))
                PAF_GameManager.Instance?.SetPlayerReady(isPlayerOne);

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
            canAttack && !stunned) Interact();
    }

    void Move()
    {
        if (!IsReady) return;
        if (!stunned)
        {
            Vector3 _dir = new Vector3(Input.GetAxis(isPlayerOne ? "Horizontal1" : "Horizontal2"), 0, Input.GetAxis(isPlayerOne ? "Vertical1" : "Vertical2"));
            if (idle = _dir.magnitude < .1f)
            {
                playerAnimator.SetMoving(idle);
                accelerationTimer = 0; 
                return;
            }
            Vector3 _nextPos = transform.position + _dir;
            _nextPos.y = moveArea.bounds.center.y;
            if (moveArea.bounds.Contains(_nextPos)
                && !Physics.Raycast(transform.position, transform.forward, 1.5f, obstacleLayer) 
                && !Physics.Raycast(transform.position, transform.forward + transform.right * .5f, 1.5f, obstacleLayer) 
                && !Physics.Raycast(transform.position, transform.forward + -transform.right * .5f, 1.5f, obstacleLayer) 
                && !falling)
            {
                accelerationTimer += Time.deltaTime;
                accelerationTimer = Mathf.Clamp(accelerationTimer, 0, 1); 
                _nextPos.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, _nextPos, Time.deltaTime * (playerSpeed * 3 * accelerationCurve.Evaluate(accelerationTimer)));
            }
            if (!Physics.Raycast(transform.position + (transform.forward + transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer) &&
                !Physics.Raycast(transform.position + (transform.forward - transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer) &&
                !Physics.Raycast(transform.position - (transform.forward + transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer) &&
                !Physics.Raycast(transform.position - (transform.forward - transform.right) * fallDetectionSize, Vector3.down, 2, groundLayer) && !falling)
            {
                falling = true;
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetFallPlayer();
                if(_clip) audioPlayer.PlayOneShot(_clip);
                //PAF_SoundManager.I.PlayFallSound(transform.position);
                playerAnimator.SetFalling();
                playerAnimator.SetMoving(false);
                transform.rotation = Quaternion.LookRotation(_dir);
                return;
            }
            transform.rotation = Quaternion.LookRotation(_dir);
        }
        else idle = true;
        if(!falling) playerAnimator.SetMoving(idle);
    }
   
    void Interact()
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
                if (_clip) audioPlayer.PlayOneShot(_clip);
            }
            PAF_Bulb _bulb = _hit.transform.GetComponent<PAF_Bulb>();
            if(_bulb)
            {
                _bulb.Hit();
            }
            PAF_Player _player = _hit.transform.GetComponent<PAF_Player>();
            if (_player && _player != this)
            {
                _player.Stun(transform.position);
            }
            if(_hit.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetHitWall();
                if (_clip) audioPlayer.PlayOneShot(_clip);
            }
        }
        AudioClip _clipSwipe = PAF_GameManager.Instance?.SoundDatas.GetHitNone();
        if (_clipSwipe) audioPlayer.PlayOneShot(_clipSwipe);
        playerAnimator.SetAttack();
        StartCoroutine(InvertBoolDelay((state) => { canAttack = state; }, attackDelay));
    }

    public void Stun(Vector3 _from)
    {
        if (!IsReady || isInvulnerable) return;
        transform.rotation = Quaternion.LookRotation(_from - transform.position);
        StartCoroutine(StunFlashTimer());
        InvokeRepeating("Flash", 0, .1f);
        AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetHitPlayer();
        if (_clip) audioPlayer.PlayOneShot(_clip);
        //PAF_SoundManager.I.PlayHitPlayer(transform.position);
        playerAnimator.SetStunned();
    }

    void Flash() => playerRenderer.enabled = !playerRenderer.enabled;

    IEnumerator StunFlashTimer()
    {
        isInvulnerable = true;
        stunned = true;
        StartCoroutine(InvertBoolDelay((state) => { isInvulnerable = !state; }, invulnerableTime + stunTime));
        yield return new WaitForSeconds(stunTime + invulnerableTime);
        CancelInvoke("Flash");
        if (playerRenderer) playerRenderer.enabled = true;
         stunned = false;
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
        if (PAF_DalleManager.I.AllUpDalles.Count <= 0) return;
        MeshCollider _dalle = PAF_DalleManager.I.AllUpDalles[Random.Range(0, PAF_DalleManager.I.AllUpDalles.Count)].GetComponent<MeshCollider>();
        Vector3 _spawnPos = new Vector3(Random.Range(_dalle.bounds.min.x, _dalle.bounds.max.x), 0, Random.Range(_dalle.bounds.min.z, _dalle.bounds.max.z));
        transform.position = _spawnPos;
        falling = false;
    }
}
