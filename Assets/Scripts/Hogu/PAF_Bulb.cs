using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PAF_Bulb : MonoBehaviour
{
    #region Object
    [SerializeField] Animator bulbAnimator = null;
    [SerializeField] GameObject item = null;
    #endregion

    #region Fields
    bool isBigBulb = false;
    [SerializeField, Header("Big bulb stats")] int bigBulbHitNeeded = 10;
    [SerializeField, Range(0, 10)] int minItemsInBigBulb = 1;
    [SerializeField, Range(1, 50)] int maxItemsInBigBulb = 10;

    [SerializeField, Header("Bulb stats"), Range(0, 10)] int minItemsInBulb = 1;
    [SerializeField, Range(1, 20)] int maxItemsInBulb = 10;

    Coroutine delayHitCoroutine = null;

    int hits = 0;
    [SerializeField] bool canHit = false;
    #endregion


    private void Start()
    {
        transform.localScale = Vector3.zero;
        if (maxItemsInBigBulb <= minItemsInBigBulb) maxItemsInBigBulb = minItemsInBigBulb + 1;
        if (maxItemsInBulb <= minItemsInBulb) maxItemsInBulb = minItemsInBulb + 1;
        delayHitCoroutine = StartCoroutine(DelayHitBulb(bulbAnimator.runtimeAnimatorController.animationClips[0].averageDuration));
    }


    public void SetBigBulb()
    {
        if (!bulbAnimator) return;
        if (delayHitCoroutine != null) StopCoroutine(delayHitCoroutine);
        bulbAnimator.SetBool("bigbulb", true);
        isBigBulb = true;
        bulbAnimator.enabled = true;
        StartCoroutine(DelayHitBulb(bulbAnimator.runtimeAnimatorController.animationClips[1].averageDuration));
    }
    
    IEnumerator DelayHitBulb(float _time)
    {
        canHit = false;
        yield return new WaitForSeconds(_time);
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
        if (!item) return;
        for (int i = 0; i < _itemsToSpawn; i++)
        {
            PAF_Fruit _fruit = Instantiate(item).GetComponent<PAF_Fruit>();
            Vector3 _force = new Vector3(Random.Range(0,1f), 0, Random.Range(0,1f));
            if (_fruit) _fruit.AddForce(_force);
        }
        Destroy(this.gameObject);
    }

}
