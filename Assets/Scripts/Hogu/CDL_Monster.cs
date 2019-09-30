using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_Monster : MonoBehaviour
{
    public static CDL_Monster I { get; private set; }

    [SerializeField] ItemType wantedItemType;
    public ItemType WantedItemType { get { return wantedItemType; } }

    // DEBUG
    [SerializeField] Renderer ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug = null;
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
        wantedItemType = (ItemType)Random.Range(0, System.Enum.GetNames(typeof(ItemType)).Length);
        // DEBUG
        switch (wantedItemType)
        {
            case ItemType.Fish:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = Color.cyan;
                break;
            case ItemType.Bone:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = Color.white;
                break;
            case ItemType.Nem:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = Color.green;
                break;
            case ItemType.Sushi:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = Color.black;
                break;
            case ItemType.Ramen:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = Color.yellow;
                break;
            case ItemType.Chicken:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = new Color(255, 192, 203);
                break;
            case ItemType.Cake:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = Color.magenta;
                break;
            case ItemType.Cookie:
                ouaisVoilaMaSuperVariableOuJaiPasLeNomDuCoupJeFaisUnTrucHyperLongEtInutileMaisOSEFLaulParceQueJeMenSersQueEnDebug.material.color = new Color(139, 69, 16);
                break;
            default:
                break;
        }
        //
    }

}
