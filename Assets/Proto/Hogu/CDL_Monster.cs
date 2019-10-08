using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_Monster : MonoBehaviour
{
    public static CDL_Monster I { get; private set; }

    [SerializeField] ItemType wantedItemType;
        public ItemType WantedItemType { get { return wantedItemType; } }

    // DEBUG
    [SerializeField] Renderer[] heads = new Renderer[4];
    //

    
    
    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        GetItem();
    }

    public void GetItem()
    {
        wantedItemType = (ItemType)Random.Range(1, System.Enum.GetNames(typeof(ItemType)).Length);
        // DEBUG
        int _rnd = Random.Range(0, 4);
        Color _color = new Color();
        switch (wantedItemType)
        {
            case ItemType.Fish:
                _color = Color.cyan;
                break;
            case ItemType.Bone:
                _color = Color.white;
                break;
            case ItemType.Nem:
                _color = Color.green;
                break;
            case ItemType.Sushi:
                _color = Color.black;
                break;
            case ItemType.Ramen:
                _color = Color.yellow;
                break;
            case ItemType.Chicken:
                _color = new Color(1, .75f, .79f);
                break;
            case ItemType.Cake:
                _color = Color.magenta;
                break;
            case ItemType.Cookie:
                _color = new Color(.54f, .27f, .06f);
                break;
            default:
                break;
        }
        heads[_rnd].GetComponent<CDL_MonsterHead>().SetDelivery(true, wantedItemType, _color);
        for (int i = 0; i < heads.Length; i++)
        {
            if(i != _rnd) heads[i].GetComponent<CDL_MonsterHead>().SetDelivery(false, ItemType.None, Color.grey);
        }
        //
    }

}
