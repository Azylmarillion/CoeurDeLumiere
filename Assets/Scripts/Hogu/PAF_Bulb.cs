using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PAF_Bulb : MonoBehaviour
{
    #region Object
    [SerializeField] Animator bulbAnimator = null;
    // [SerializeField] GameObject item = null;
    #endregion

    #region Fields
    bool isBigBulb = false;
    [SerializeField, Header("Big bulb stats")] int bigBulbHitNeeded = 10;
    [SerializeField, Range(0, 10)] int minItemsInBigBulb = 1;
    [SerializeField, Range(1, 50)] int maxItemsInBigBulb = 10;

    [SerializeField, Header("Bulb stats"), Range(0, 10)] int minItemsInBulb = 1;
    [SerializeField, Range(1, 20)] int maxItemsInBulb = 10;

    int hits = 0;
    bool canHit = true;
    #endregion


    private void Start()
    {
        if (maxItemsInBigBulb <= minItemsInBigBulb) maxItemsInBigBulb = minItemsInBigBulb + 1;
        if (maxItemsInBulb <= minItemsInBulb) maxItemsInBulb = minItemsInBulb + 1;
    }



    public void SetBigBulb()
    {
        if (!bulbAnimator) return;
        isBigBulb = true;
        bulbAnimator.enabled = true;
        StartCoroutine(DelayHitBigBulb());
    }
    
    IEnumerator DelayHitBigBulb()
    {
        canHit = false;
        yield return new WaitForSeconds(bulbAnimator.runtimeAnimatorController.animationClips[0].averageDuration);
        canHit = true;
    }

    public void Hit()
    {
        if (isBigBulb && canHit)
        {
            hits++;
            if (hits >= bigBulbHitNeeded)
            {
                int _rnd = Random.Range(minItemsInBigBulb, maxItemsInBigBulb);
                Explode(_rnd);
            }
        }
        else
        {
            int _rnd = Random.Range(minItemsInBulb, maxItemsInBulb);
            Explode(_rnd);
        }
    }

    void Explode(int _itemsToSpawn)
    {
        // if(!item) return;
        for (int i = 0; i < _itemsToSpawn; i++)
        {
            // PAF_Item _item = Instantiate(item).GetComponent<PAF_Item>();
            // Vector3 _force = new Vector3(Random.Range(), 0, Random.Range());
            // if(_item) _item.AddForce(_force);
        }
        Destroy(this.gameObject);
    }

}
