using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_MonsterHead : MonoBehaviour
{
    [SerializeField] bool isDelivery = false;
    [SerializeField] ItemType wantedItem = ItemType.None;
    [SerializeField] Collider hitbox = null;
    [SerializeField] Renderer headRenderer = null;

    private void OnTriggerEnter(Collider other)
    {
        CDL_Player _pl = other.GetComponent<CDL_Player>();
        if(_pl)
        {
            if(_pl.GrabbedItem && _pl.GrabbedItem.CurrentItemType == wantedItem)
            {
                CDL_Monster.I.GetItem();
                _pl.GrabbedItem.Init();
                _pl.GrabItem();
                //CDL_GameManager.I.Win(_pl);
            }
        }
    }

    public void SetDelivery(bool _state, ItemType _type, Color _color)
    {
        headRenderer.material.color = _color;
        wantedItem = _type;
        isDelivery = _state;
        hitbox.enabled = _state;

    }


}
