using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Player : MonoBehaviour
{
    #region Fields
    [SerializeField] bool isPlayerOne = true;
        public bool IsPlayerOne { get { return isPlayerOne; } }
    [SerializeField] bool stunned = false;
    bool isInvulnerable = false;
    [SerializeField, Range(0, 5)] float invulnerableTime = .5f;
    [SerializeField, Range(0, 5)] float stunTime = .5f;

    [SerializeField, Range(0, 5)] float playerSpeed = 1.5f;

    [Header("Sight")]
    [SerializeField, Range(2, 180)] int sightAngle = 45;
    [SerializeField, Range(0, 5)] float sightRange = 1.5f;
    #endregion

    #region Objects
    [SerializeField] Collider moveArea = null;
    [SerializeField] Renderer playerRenderer = null;
    #endregion

    #region Layers
    [Space, Header("Layers")]
    [SerializeField] LayerMask wallLayer = 0;
    [SerializeField] LayerMask itemLayer = 0;
    [SerializeField] LayerMask playerLayer = 0;
    [SerializeField] LayerMask bulbLayer = 0;
    #endregion

    private void Update()
    {
        Move();
        if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0) ||
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1) ||
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2) ||
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3)) Interact();
    }

    private void OnDrawGizmos()
    {
        for (int i = -(sightAngle / 2); i < sightAngle / 2; i += 10)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, i, 0) * transform.forward).normalized * sightRange);
        }
    }


    void Move()
    {
        if (!stunned)
        {
            Vector3 _dir = new Vector3(Input.GetAxisRaw(isPlayerOne ? "Horizontal1" : "Horizontal2"), 0, Input.GetAxisRaw(isPlayerOne ? "Vertical1" : "Vertical2"));
            if (_dir.magnitude < .1f) return;
            Vector3 _nextPos = transform.position + _dir;
            if (moveArea.bounds.Contains(_nextPos) && !Physics.Raycast(transform.position, transform.forward, 1.5f, wallLayer))
            {
                transform.position = Vector3.MoveTowards(transform.position, _nextPos, Time.deltaTime * (playerSpeed * 3));
            }
            transform.rotation = Quaternion.LookRotation(_dir);
        }
    }
     
    void Interact()
    {
        for (int i = -(sightAngle / 2); i < sightAngle / 2; i += 10)
        {
            RaycastHit[] _hitItems = Physics.RaycastAll(transform.position, (Quaternion.Euler(0, i, 0) * transform.forward).normalized, sightRange, itemLayer);
            foreach (RaycastHit _hit in _hitItems)
            {
                CDL_Item _item = _hit.transform.GetComponent<CDL_Item>(); // A CHANGER EN PAF_Item QUAND IL SERA FAIT
                if (_item) _item.Kick();
            }
        }
        for (int i = -(sightAngle / 2); i < sightAngle / 2; i += 10)
        {
            RaycastHit _hitPlayer;
            if (Physics.Raycast(transform.position, (Quaternion.Euler(0, i, 0) * transform.forward).normalized, out _hitPlayer, sightRange, playerLayer))
            {
                PAF_Player _player = _hitPlayer.transform.GetComponent<PAF_Player>();
                if (_player) _player.Stun();
                break;
            }
        }
        for (int i = -(sightAngle / 2); i < sightAngle / 2; i += 10)
        {
            RaycastHit _hitBulb;
            if (Physics.Raycast(transform.position, (Quaternion.Euler(0, i, 0) * transform.forward).normalized, out _hitBulb, sightRange, bulbLayer))
            {
                PAF_Bulb _bulb = _hitBulb.transform.GetComponent<PAF_Bulb>();
                if (_bulb) _bulb.Hit();
                break;
            }
        }
    }

    public void Stun()
    {
        StartCoroutine(StunFlashTimer());
        InvokeRepeating("Flash", 0, .1f);
    }

    void Flash()
    {
        if (!playerRenderer) return;
        playerRenderer.enabled = !playerRenderer.enabled;
    }

    IEnumerator StunFlashTimer()
    {
        isInvulnerable = true;
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        CancelInvoke("Flash");
        if (playerRenderer) playerRenderer.enabled = true;
         stunned = false;
        StartCoroutine(InvulnerableDelay());
    }

    IEnumerator InvulnerableDelay()
    {
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
    }

}
