using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CDL_Player : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] bool isPlayerOne = true;
        public bool IsPlayerOne { get { return isPlayerOne; } }
    bool stunned = false;
    bool isInvulnerable = false;
    [SerializeField, Range(0, 5)] float invulnerableTime = .5f;
    [SerializeField, Range(0, 5)] float stunTime = .5f;
    [Header("Sight")]
    [SerializeField, Range(2, 180)] int sightAngle = 45;
    [SerializeField, Range(0, 5)] float sightRange = 1.5f;

    [Space, Header("Objects")]
    [SerializeField] Collider moveArea = null;
    [SerializeField] Renderer playerRenderer = null;
    
    [Space, Header("Layers")]
    [SerializeField] LayerMask wallLayer = 0;
    [SerializeField] LayerMask itemLayer = 0;
    [SerializeField] LayerMask playerLayer = 0;
    [SerializeField] LayerMask bulbLayer = 0;
    

    //DEBUG
    private void Start()
    {
        for (int i = 0; i < playerRenderer.materials.Count(); i++)
        {
            playerRenderer.materials[i].color = isPlayerOne ? Color.blue : Color.red;
        }
    }
    //



    void Update()
    {
        Move();
        if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0)|| 
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1)|| 
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2)|| 
            Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3)) Interact();
    }



    private void OnDrawGizmos()
    {
        for (int i = -(sightAngle / 2); i < sightAngle / 2; i+=10)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, ((Quaternion.Euler(0, i, 0) * transform.forward).normalized) * sightRange);
        }
    }


    void Move()
    {
        if (!stunned)
        {
            Vector3 _pos = transform.position;
            Vector3 _dir = new Vector3(Input.GetAxis(isPlayerOne ? "Horizontal1" : "Horizontal2"), 0, Input.GetAxis(isPlayerOne ? "Vertical1" : "Vertical2")) *.1f;
            Vector3 _nextPos = _pos + _dir;
            if (moveArea.bounds.Contains(_nextPos) && !Physics.Raycast(transform.position, transform.forward, 1.5f, wallLayer))
            {
                transform.position += _dir;
            }
            if(_dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(_dir);
        }
    }

    void Interact()
    {
        for (int i = -(sightAngle / 2); i < sightAngle / 2; i+=10)
        {
            RaycastHit[] _hitItems = Physics.RaycastAll(transform.position, (Quaternion.Euler(0, i, 0) * transform.forward).normalized, sightRange, itemLayer);
            RaycastHit _hitPlayer;
            if(Physics.Raycast(transform.position, transform.forward + transform.TransformDirection(new Vector3(i, 0, 0)), out _hitPlayer, sightRange, playerLayer)) if(_hitPlayer.transform.GetComponent<CDL_Player>())  _hitPlayer.transform.GetComponent<CDL_Player>().Stun();
            RaycastHit _hitBulb;
            if(Physics.Raycast(transform.position, transform.forward + transform.TransformDirection(new Vector3(i, 0, 0)), out _hitBulb, sightRange, bulbLayer)) if (_hitPlayer.transform.GetComponent<CDL_Bulb>()) _hitPlayer.transform.GetComponent<CDL_Bulb>().Hit();
            foreach (RaycastHit _hit in _hitItems)
            {
                CDL_Item _item = _hit.transform.GetComponent<CDL_Item>();
                if (_item) _item.Kick();
            }
        }
    }
    
    
    public void Stun()
    {
        if (!playerRenderer || isInvulnerable) return;
        StartCoroutine(StunFlashTimer());
        InvokeRepeating("Flash", 0, .1f);
    }

    void Flash()
    {
        playerRenderer.enabled = !playerRenderer.enabled;
    }

    IEnumerator StunFlashTimer()
    {
        isInvulnerable = true;
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        CancelInvoke("Flash");
        playerRenderer.enabled = true;
        stunned = false;
        StartCoroutine(InvulnerableDelay());
    }

    IEnumerator InvulnerableDelay()
    {
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
    }

    
}
