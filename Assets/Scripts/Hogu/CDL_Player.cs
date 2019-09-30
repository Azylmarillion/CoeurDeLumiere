using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CDL_Player : MonoBehaviour
{
    [SerializeField] bool isPlayerOne = true;


    [SerializeField] Renderer playerRenderer = null;
    [SerializeField] bool stunned = false;

    [SerializeField] List<CDL_Item> nearbyItems = new List<CDL_Item>();
    [SerializeField] CDL_Item grabbedItem = null;

    void Update()
    {
        Move();
        if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3)) GrabItem();
        if (Input.GetKeyDown(isPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2)) PunchThrow();
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
            CDL_WindowManager.I.SwitchDelivery(true);
            grabbedItem.SetGrabbed(true, this);
        }
        else
        {
            grabbedItem.SetGrabbed(false);
            CDL_WindowManager.I.SwitchDelivery(false);
            grabbedItem = null;
        }
    }

    void PunchThrow()
    {
        if(grabbedItem)
        {
            grabbedItem.Throw();
            CDL_WindowManager.I.SwitchDelivery(false);
            grabbedItem = null;
        }
        else
        {
            // do punch
        }
    }

    public void GetStunnedFam(float _stunTime)
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
}
