using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CDL_Player : MonoBehaviour
{
    [SerializeField] bool isPlayerOne = true;
        public bool IsPlayerOne { get { return isPlayerOne; } }


    [SerializeField] Renderer playerRenderer = null;
    [SerializeField] bool stunned = false;

    [SerializeField] List<CDL_Item> nearbyItems = new List<CDL_Item>();
    [SerializeField] CDL_Item grabbedItem = null;

    [SerializeField] int dashCurrentCharges = 1;
    [SerializeField] int dashMaxCharges = 1;
    [SerializeField] float dashReloadtime = 5;
    float dashReloadTimer = 0;

    [SerializeField] CDL_Punch playerPunch = null;

    void Update()
    {
        Move();
        if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3)) GrabItem();
        if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2)) PunchThrow();
        if(!stunned && dashCurrentCharges > 0) if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button4 : KeyCode.Joystick2Button4) || Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button5 : KeyCode.Joystick2Button5)) Dash();
        if (dashCurrentCharges < dashMaxCharges) ReloadDash();
    }
    



    void Move()
    {
        if (!stunned)
        {
            Vector3 _pos = transform.position;
            transform.position += (new Vector3(Input.GetAxis(isPlayerOne ? "Horizontal1" : "Horizontal2"), 0, Input.GetAxis(isPlayerOne ? "Vertical1" : "Vertical2"))) * .1f;
            if(_pos != transform.position) transform.rotation = Quaternion.LookRotation(transform.position - _pos);
        }
    }

    public void SetItemNearby(CDL_Item _item, bool _isClose)
    {
        if (_isClose && !nearbyItems.Contains(_item)) nearbyItems.Add(_item);
        else if (!_isClose && nearbyItems.Contains(_item)) nearbyItems.Remove(_item);
    }

    void GrabItem()
    {
        if (!grabbedItem)
        {
            if (nearbyItems.Count < 1) return;
            grabbedItem = nearbyItems.Where(i => i.CurrentItemType == CDL_Monster.I.WantedItemType).FirstOrDefault();
            if (!grabbedItem) grabbedItem = nearbyItems[Random.Range(0, nearbyItems.Count)];
            //CDL_WindowManager.I.SwitchDelivery(true);
            grabbedItem.SetGrabbed(true, this);
        }
        else
        {
            grabbedItem.SetGrabbed(false);
            //CDL_WindowManager.I.SwitchDelivery(false);
            grabbedItem = null;
        }
    }

    void PunchThrow()
    {
        if(grabbedItem)
        {
            grabbedItem.Throw();
            //CDL_WindowManager.I.SwitchDelivery(false);
            grabbedItem = null;
        }
        else
        {
            if (playerPunch) playerPunch.Punch();
        }
    }

    public void Stun(float _stunTime)
    {
        if (!playerRenderer) return;
        StartCoroutine(StunFlashTimer(_stunTime));
        InvokeRepeating("Flash", 0, .1f);
    }

    void Flash()
    {
        playerRenderer.enabled = !playerRenderer.enabled;
    }

    IEnumerator StunFlashTimer(float _stunTime)
    {
        stunned = true;
        yield return new WaitForSeconds(_stunTime);
        CancelInvoke("Flash");
        playerRenderer.enabled = true;
        stunned = false;
    }


    void Dash()
    {
        dashCurrentCharges--;
        stunned = true;
        InvokeRepeating("RepeatDash", 0, .05f);
        StartCoroutine(DelayDash());
    }

    void RepeatDash()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * 100);
    }

    IEnumerator DelayDash()
    {
        yield return new WaitForSeconds(.1f);
        CancelInvoke("RepeatDash");
        stunned = false;
    }

    void ReloadDash()
    {
        dashReloadTimer += Time.deltaTime;
        if(dashReloadTimer >= dashReloadtime)
        {
            dashCurrentCharges++;
            if (dashCurrentCharges > dashMaxCharges) dashCurrentCharges = dashMaxCharges;
            dashReloadTimer = 0;
        }
    }
}
