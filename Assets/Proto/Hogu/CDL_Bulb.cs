using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_Bulb : MonoBehaviour
{
    [SerializeField, Range(0, 10)] int minItemsSpawn = 1; 
    [SerializeField, Range(1, 20)] int maxItemsSpawn = 5;

    [SerializeField] GameObject item = null;

    [SerializeField] bool isBigBulb = false;
    [SerializeField, Range(0, 20)] int numberOfHit = 5;
    [SerializeField] int hits = 0;


    private void Start()
    {
        if (maxItemsSpawn <= minItemsSpawn) maxItemsSpawn = minItemsSpawn + 1;
    }


    public void Hit()
    {
        //if (!item) return;
        if (isBigBulb)
        {
            hits++;
            if (hits >= numberOfHit) BigBulbExplode();
        }
        else
        {
            int _rnd = Random.Range(minItemsSpawn, maxItemsSpawn);
            for (int i = 0; i < _rnd; i++)
            {
                Instantiate(item, transform.position, item.transform.rotation);
            }
        }
    }

    void BigBulbExplode()
    {

    }
}
