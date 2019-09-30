﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Test_Player : MonoBehaviour
{
    [SerializeField] Renderer playerRenderer = null;
    [SerializeField] bool stunned = false;

    [SerializeField] List<CDL_Item> nearbyItems = new List<CDL_Item>();
    [SerializeField] CDL_Item grabbedItem = null;

    void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Z)) GrabItem();
        
    }

    void Move()
    {
        if(!stunned) transform.position += (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * .1f;
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
            else CDL_WindowManager.I.SwitchDelivery(true);
            //grabbedItem.SetGrabbed(true, this);
        }
        else
        {
            grabbedItem.SetGrabbed(false);
            if (grabbedItem.CurrentItemType == CDL_Monster.I.WantedItemType) CDL_WindowManager.I.SwitchDelivery(false);
            grabbedItem = null;
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
