using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PAF_Bulb : MonoBehaviour
{
    #region Object
    [SerializeField] Animator bulbAnimator = null;
    [SerializeField] GameObject[] items = null;
    [SerializeField] PAF_SoundData soundData = null;
    [SerializeField] AudioSource soundSource = null;
    [SerializeField] PAF_FruitData fruitData = null;
    #endregion

    #region Fields
    [SerializeField] bool isBigBulb = false;
    [SerializeField, Header("Big bulb stats")] int bigBulbHitNeeded = 10;
    [SerializeField, Range(0, 10)] int minItemsInBigBulb = 1;
    [SerializeField, Range(1, 50)] int maxItemsInBigBulb = 10;

    [SerializeField, Header("Bulb stats"), Range(0, 10)] int minItemsInBulb = 1;
    [SerializeField, Range(1, 20)] int maxItemsInBulb = 10;

    Coroutine delayHitCoroutine = null;

    int hits = 0;
    bool canHit = false;

    [Header("Hit force")]
    [SerializeField, Range(0, 100)] float minHitForce = 10; 
    [SerializeField, Range(1, 100)] float maxHitForce = 50; 
    [SerializeField, Range(0, 100)] float minHeightForce = 10; 
    [SerializeField, Range(1, 100)] float maxHeightForce = 50;
    #endregion
    

    private void Start()
    {
        transform.localScale = Vector3.zero;
        if (maxItemsInBigBulb <= minItemsInBigBulb) maxItemsInBigBulb = minItemsInBigBulb + 1;
        if (maxItemsInBulb <= minItemsInBulb) maxItemsInBulb = minItemsInBulb + 1;
        if (maxHitForce <= minHitForce) maxHitForce = minHitForce + 1;
        if (maxHeightForce <= minHeightForce) maxHeightForce = minHeightForce + 1;
        if(bulbAnimator) delayHitCoroutine = StartCoroutine(PAF_Player.InvertBoolDelay((state) => { canHit = state; }, bulbAnimator.runtimeAnimatorController.animationClips[0].averageDuration));
        if (isBigBulb) SetBigBulb();
    }


    public void SetBigBulb()
    {
        if (!bulbAnimator) return;
        if (delayHitCoroutine != null) StopCoroutine(delayHitCoroutine);
        bulbAnimator.SetBool("bigbulb", true);
        isBigBulb = true;
        bulbAnimator.enabled = true;
        StartCoroutine(PAF_Player.InvertBoolDelay((state) => { canHit = state; }, bulbAnimator.runtimeAnimatorController.animationClips[1].averageDuration));
    }
    
    public void Hit()
    {
        if (soundData && soundSource)
        {
            AudioClip _clip = soundData.GetHitBulb();
            if (_clip) soundSource.PlayOneShot(_clip);
        }
        if (isBigBulb && canHit)
        {
            hits++;
            if (hits >= bigBulbHitNeeded)
            {
                int _rnd = Random.Range(minItemsInBigBulb, maxItemsInBigBulb);
                Explode(_rnd);
            }
        }
        else if (canHit)
        {
            int _rnd = Random.Range(minItemsInBulb, maxItemsInBulb);
            Explode(_rnd);
        }
    }

    public void Explode(int _itemsToSpawn)
    {
        if (items.Length < 0 || !bulbAnimator || !fruitData) return;
        items = fruitData.GetRandomFruit(Random.Range(isBigBulb ? minItemsInBigBulb : minItemsInBulb, isBigBulb ? maxItemsInBigBulb : maxItemsInBulb));
        for (int i = 0; i < _itemsToSpawn; i++)
        {
            if (soundData && soundSource)
            {
                AudioClip _clip = soundData.GetFruitSpawn();
                if (_clip) soundSource.PlayOneShot(_clip);
            }
            PAF_Fruit _fruit = Instantiate(items[Random.Range(0, items.Length)], transform.position, transform.rotation).GetComponent<PAF_Fruit>();
            Vector3 _force = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            _force = Vector3.ClampMagnitude(_force, Random.Range(.1f, 1));
            if (_fruit) _fruit.AddForce(_force);
        }
        bulbAnimator.SetBool("explode", true);
        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        if (soundData && soundSource)
        {
            AudioClip _clip = soundData.GetBulbExploding();
            if (_clip) soundSource.PlayOneShot(_clip);
        }
        yield return new WaitForSeconds(bulbAnimator.runtimeAnimatorController.animationClips[isBigBulb ? 2 : 3].averageDuration);
        //PAF_SoundManager.I.PlayBulbExplode(transform.position);
        Destroy(this.gameObject);
    }
}
