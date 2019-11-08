using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PAF_Bulb : MonoBehaviour
{
    public event Action OnBulbDestroyed; 

    #region Object
    [SerializeField] Animator bulbAnimator = null;
    [SerializeField] GameObject[] items = null;
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



    Vector3 initScale = Vector3.zero;
    bool animateBigBulb = false;
    #endregion


    private void Start()
    {
        initScale = transform.localScale;
        if (maxItemsInBigBulb <= minItemsInBigBulb) maxItemsInBigBulb = minItemsInBigBulb + 1;
        if (maxItemsInBulb <= minItemsInBulb) maxItemsInBulb = minItemsInBulb + 1;
        if (maxHitForce <= minHitForce) maxHitForce = minHitForce + 1;
        if (maxHeightForce <= minHeightForce) maxHeightForce = minHeightForce + 1;
        canHit = false;
        if (isBigBulb) SetBigBulb();
    }

    private void Update()
    {
        if(animateBigBulb) AnimateBigger();
    }



    public void SetBigBulb()
    {
        if (!bulbAnimator) return;
        isBigBulb = true;
        bulbAnimator.enabled = true;
        canHit = false;
        StartCoroutine(BigBulbAnimation());
    }

    void AnimateBigger()
    {
        transform.localScale = Vector3.Slerp(transform.localScale, initScale + Vector3.one * .5f, Time.deltaTime * 20);
    }

    IEnumerator BigBulbAnimation()
    {
        animateBigBulb = true;
        bulbAnimator.SetTrigger("spawn");
        yield return new WaitForSeconds(.5f);
        animateBigBulb = false;
        initScale = transform.localScale;
        yield return new WaitForSeconds(2);
        if (soundSource)
        {
            AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetBulbExploding();
            if (_clip) soundSource.PlayOneShot(_clip);
        }
        animateBigBulb = true;
        bulbAnimator.SetTrigger("spawn");
        yield return new WaitForSeconds(.5f);
        animateBigBulb = false;
        initScale = transform.localScale;
        yield return new WaitForSeconds(2);
        if (soundSource)
        {
            AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetBulbExploding();
            if (_clip) soundSource.PlayOneShot(_clip);
        }
        animateBigBulb = true;
        bulbAnimator.SetTrigger("spawn");
        yield return new WaitForSeconds(.5f);
        animateBigBulb = false;
        initScale = transform.localScale;
        yield return new WaitForSeconds(2);
        canHit = true;
    }
    
    public void Hit()
    {
        if (!bulbAnimator) return;
        bulbAnimator.SetTrigger("hit");
        if (soundSource)
        {
            AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetHitBulb();
            if (_clip) soundSource.PlayOneShot(_clip);
        }
        if (isBigBulb && canHit)
        {
            hits++;
            if (hits >= bigBulbHitNeeded)
            {
                bulbAnimator.SetTrigger("spit");
            }
        }
        else if (canHit)
        {
            bulbAnimator.SetTrigger("spit");
        }
    }

    public void Explode(bool _spawnItems)
    {
        if (items.Length < 0 || !bulbAnimator || !fruitData) return;
        int _itemsToSpawn = Random.Range(isBigBulb ? minItemsInBigBulb : minItemsInBulb, isBigBulb ? maxItemsInBigBulb : maxItemsInBulb);
        if (!_spawnItems) _itemsToSpawn = 0;
        items = fruitData.GetRandomFruit(Random.Range(isBigBulb ? minItemsInBigBulb : minItemsInBulb, isBigBulb ? maxItemsInBigBulb : maxItemsInBulb));
        for (int i = 0; i < _itemsToSpawn; i++)
        {
            if (soundSource)
            {
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetFruitSpawn();
                if (_clip) soundSource.PlayOneShot(_clip);
            }
            PAF_Fruit _fruit = Instantiate(items[Random.Range(0, items.Length)], transform.position, transform.rotation).GetComponent<PAF_Fruit>();
            Vector3 _force = new Vector3(Random.Range(-5, 5f), Random.Range(.1f, 5f), Random.Range(-5, 5f));
            //_force = Vector3.ClampMagnitude(_force, Random.Range(.1f, 1));
            if (_fruit) _fruit.AddForce(_force);
        }
        bulbAnimator.SetTrigger("explode");
    }
    
    public void ExplodeWithoutBool()
    {
        if (items.Length < 0 || !bulbAnimator || !fruitData) return;
        int _itemsToSpawn = Random.Range(isBigBulb ? minItemsInBigBulb : minItemsInBulb, isBigBulb ? maxItemsInBigBulb : maxItemsInBulb);
        items = fruitData.GetRandomFruit(Random.Range(isBigBulb ? minItemsInBigBulb : minItemsInBulb, isBigBulb ? maxItemsInBigBulb : maxItemsInBulb));
        for (int i = 0; i < _itemsToSpawn; i++)
        {
            if (soundSource)
            {
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetFruitSpawn();
                if (_clip) soundSource.PlayOneShot(_clip);
            }
            PAF_Fruit _fruit = Instantiate(items[Random.Range(0, items.Length)], transform.position, transform.rotation).GetComponent<PAF_Fruit>();
            Vector3 _force = new Vector3(Random.Range(-50, 50f), Random.Range(.1f, 5f), Random.Range(-50, 50f));
            //_force = Vector3.ClampMagnitude(_force, Random.Range(.1f, 1));
            if (_fruit) _fruit.AddForce(_force);
        }
        bulbAnimator.SetTrigger("explode");
    }


    public void SetCanHit(bool _state) => canHit = _state;

    public void DestroyBulb()
    {
        OnBulbDestroyed?.Invoke();
        OnBulbDestroyed = null; 
        if (soundSource)
        {
            AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetBulbExploding();
            if (_clip) soundSource.PlayOneShot(_clip);
        }
    }
}
