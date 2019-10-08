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
        CDL_Item _it = other.GetComponent<CDL_Item>();
        if(_it)
        {
            // _it.FaireQuelqueChoseGenreUneMethodeQuiSappeleraisEatParExemple();
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
