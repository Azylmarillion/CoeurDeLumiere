using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PAF_Bulb : MonoBehaviour
{
    public event Action OnBulbDestroyed; 

    #region Object
    [SerializeField] Animator bulbAnimator = null;
    [SerializeField] GameObject[] items = null;
    [SerializeField] AudioSource soundSource = null;
    [SerializeField] PAF_FruitData m_fruitData = null;
    #endregion

    #region Fields
    [SerializeField] bool isBigBulb = false;
    private static int goldenFruitsAmount = 3;
    private static int goldenFruitsRemaining = 0;

    [SerializeField, Header("Big bulb stats")] int bigBulbHitNeeded = 10;
    [SerializeField, Range(0, 10)] int minItemsInBigBulb = 1;
    [SerializeField, Range(1, 50)] int maxItemsInBigBulb = 10;

    [SerializeField, Header("Bulb stats"), Range(0, 10)] int minItemsInBulb = 1;
    [SerializeField, Range(1, 20)] int maxItemsInBulb = 10;

    int hits = 0;
    [SerializeField] bool canHit = false;

    [Header("Hit force")]
    [SerializeField, Range(0, 100)] float minHitForce = 10; 
    [SerializeField, Range(1, 100)] float maxHitForce = 50; 
    [SerializeField, Range(0, 100)] float minHeightForce = 10; 
    [SerializeField, Range(1, 100)] float maxHeightForce = 50;

    private PAF_Player m_lastHitPlayer = null; 

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

    public static void InitGoldenAmount()
    {
        goldenFruitsRemaining = goldenFruitsAmount;
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
        yield return new WaitForSeconds(.75f);
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
        yield return new WaitForSeconds(.75f);
        if (soundSource)
        {
            AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetBulbExploding();
            if (_clip) soundSource.PlayOneShot(_clip);
        }
        animateBigBulb = true;
        bulbAnimator.SetTrigger("spawn");
        canHit = true;
        yield return new WaitForSeconds(.5f);
        animateBigBulb = false;
        initScale = transform.localScale;
    }
    
    public void Hit(PAF_Player _player)
    {
        if (!bulbAnimator) return;
        m_lastHitPlayer = _player; 
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
            else
            {
                bulbAnimator.SetTrigger("hit");
            }
        }
        else if (canHit)
        {
            bulbAnimator.SetTrigger("spit");
        }
        else
        {
            bulbAnimator.SetTrigger("hit");
        }

        // Bulb FX
        ParticleSystem _system = PAF_GameManager.Instance?.VFXDatas?.BulbFX;
        if (_system) Instantiate(_system.gameObject, transform.position + Vector3.up, Quaternion.identity);
    }

    public void Explode(bool _spawnItems)
    {
        if (items.Length < 0 || !bulbAnimator || !m_fruitData) return;
        int _itemsToSpawn = 0;

        if (_spawnItems)
        {
            if (isBigBulb)
            {
                _itemsToSpawn = Random.Range(minItemsInBigBulb, maxItemsInBigBulb + 1);
                if (goldenFruitsRemaining > 0)
                {
                    items = new GameObject[] { m_fruitData.GetGoldenFruit };
                    goldenFruitsRemaining--;
                    _itemsToSpawn--;
                }
                
                items = items.Concat(m_fruitData.GetRandomFruit(_itemsToSpawn)).ToArray();
            }
            else
            {
                _itemsToSpawn = Random.Range(minItemsInBulb, maxItemsInBulb + 1);
                if ((goldenFruitsRemaining > 0) && (goldenFruitsAmount > 1))
                {
                    int _goldenFruitPercent = 0;
                    if (PAF_GameManager.Instance?.CurrentGameTimePercent < .4f)
                    {
                        if ((goldenFruitsAmount - goldenFruitsRemaining) < ((goldenFruitsAmount - 1) % 2))
                        {
                            if (PAF_GameManager.Instance.CurrentGameTimePercent < .15f) _goldenFruitPercent = 0;
                            else if (PAF_GameManager.Instance.CurrentGameTimePercent < .25f) _goldenFruitPercent = 20;
                            else _goldenFruitPercent = 0;
                        }
                    }
                    else
                    {
                        if (PAF_GameManager.Instance.CurrentGameTimePercent < .66f)
                        {
                            if (goldenFruitsRemaining > ((goldenFruitsAmount - 1) % 2f))
                            {
                                _goldenFruitPercent = 25;
                            }
                            else _goldenFruitPercent = 0;
                        }
                        else if (PAF_GameManager.Instance.CurrentGameTimePercent < .85f) _goldenFruitPercent = 15;
                        else _goldenFruitPercent = 100;
                    }

                    if (Random.Range(0, 100) < _goldenFruitPercent)
                    {
                        items = new GameObject[] { m_fruitData.GetGoldenFruit };
                        goldenFruitsRemaining--;
                        _itemsToSpawn--;
                    }
                }
                items = items.Concat(m_fruitData.GetRandomFruit(_itemsToSpawn)).ToArray();
            }
        }

        for (int i = 0; i < _itemsToSpawn; i++)
        {
            if (soundSource)
            {
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetFruitSpawn();
                if (_clip) soundSource.PlayOneShot(_clip);
            }

            float _range = isBigBulb ? 20 : 15;
            float _height = isBigBulb ? 5 : 3;

            PAF_Fruit _fruit = Instantiate(items[i], transform.position, transform.rotation).GetComponent<PAF_Fruit>();
            Vector3 _force = new Vector3(Random.Range(-_range, _range), Random.Range(.25f, _height), Random.Range(_range, _range));
            if (_fruit) _fruit.AddForce(_force, m_lastHitPlayer);
        }
        bulbAnimator.SetTrigger("explode");
    }
    
    public void ExplodeWithoutBool() => Explode(true);


    public void SetCanHit(bool _state)
    {
        if (isBigBulb) return;
        canHit = _state;
    }

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
