using UnityEngine;

using Random = UnityEngine.Random;

public class PAF_Bulb : MonoBehaviour
{
    #region Object
    [SerializeField] Animator bulbAnimator = null;
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

    private readonly int spawn_Hash = Animator.StringToHash("spawn");
    private readonly int spit_Hash = Animator.StringToHash("spit");
    private readonly int hit_Hash = Animator.StringToHash("hit");
    private readonly int explode_Hash = Animator.StringToHash("explode");

    private WaitForSeconds waitShort = new WaitForSeconds(.5f);
    private WaitForSeconds waitLong = new WaitForSeconds(.75f);

    private float bigBulbTimer = 0;
    private int bigBulbGrow = 0;

    private void Start()
    {
        initScale = transform.localScale;
        if (maxItemsInBigBulb <= minItemsInBigBulb) maxItemsInBigBulb = minItemsInBigBulb + 1;
        if (maxItemsInBulb <= minItemsInBulb) maxItemsInBulb = minItemsInBulb + 1;
        if (maxHitForce <= minHitForce) maxHitForce = minHitForce + 1;
        if (maxHeightForce <= minHeightForce) maxHeightForce = minHeightForce + 1;
        canHit = false;

        if (isBigBulb)
            SetBigBulb();
    }

    private void Update()
    {
        if (animateBigBulb)
        {
            transform.localScale = Vector3.Slerp(transform.localScale, initScale + Vector3.one * .5f, Time.deltaTime * 20);

            bigBulbTimer += Time.deltaTime;
            if (bigBulbTimer > .5f)
            {
                bigBulbTimer = 0;
                animateBigBulb = false;
                initScale = transform.localScale;
            }
        }
        else if (isBigBulb && (bigBulbGrow < 2))
        {
            bigBulbTimer += Time.deltaTime;
            if (bigBulbTimer > .75f)
            {
                bigBulbTimer = 0;
                animateBigBulb = true;
                bulbAnimator.SetTrigger(spawn_Hash);

                soundSource.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetBulbExploding());

                bigBulbGrow++;
                if (bigBulbGrow > 1)
                {
                    canHit = true;
                }
            }
        }
    }

    private void OnDestroy() => PAF_BulbManager.Instance.RemoveBulb(this);

    public static void InitGoldenAmount()
    {
        goldenFruitsRemaining = goldenFruitsAmount;
    }

    public void SetBigBulb()
    {
        isBigBulb = true;
        bulbAnimator.enabled = true;
        canHit = false;

        animateBigBulb = true;
        bulbAnimator.SetTrigger(spawn_Hash);
    }
    
    public void Hit(PAF_Player _player)
    {
        m_lastHitPlayer = _player;
        soundSource.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetHitBulb());

        if (canHit)
        {
            if (isBigBulb)
            {
                hits++;

                if (hits >= bigBulbHitNeeded)
                    bulbAnimator.SetTrigger(spit_Hash);
                else
                    bulbAnimator.SetTrigger(hit_Hash);
            }
            else
                bulbAnimator.SetTrigger(spit_Hash);
        }
        else
            bulbAnimator.SetTrigger(hit_Hash);

        // Bulb FX
        Instantiate(PAF_GameManager.Instance.VFXDatas.BulbFX.gameObject, transform.position + Vector3.up, Quaternion.identity);
    }

    public void Explode(bool _spawnItems)
    {
        if (_spawnItems)
        {
            GameObject[] _items;
            int _itemsToSpawn;

            if (isBigBulb)
            {
                _itemsToSpawn = Random.Range(minItemsInBigBulb, maxItemsInBigBulb + 1);
                _items = m_fruitData.GetRandomFruit(_itemsToSpawn);
                
                if (goldenFruitsRemaining > 0)
                {
                    goldenFruitsRemaining--;

                    _items[_itemsToSpawn] = m_fruitData.GetGoldenFruit;
                    _itemsToSpawn++;
                }
            }
            else
            {
                _itemsToSpawn = Random.Range(minItemsInBulb, maxItemsInBulb + 1);
                _items = m_fruitData.GetRandomFruit(_itemsToSpawn);

                if ((goldenFruitsRemaining > 0) && (goldenFruitsAmount > 1))
                {
                    int _goldenFruitPercent = 0;
                    if (PAF_GameManager.Instance.CurrentGameTimePercent < .4f)
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
                        goldenFruitsRemaining--;

                        _items[_itemsToSpawn] = m_fruitData.GetGoldenFruit;
                        _itemsToSpawn++;
                    }
                }
            }

            for (int i = 0; i < _itemsToSpawn; i++)
            {
                soundSource.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetFruitSpawn());

                float _range = isBigBulb ? 20 : 15;
                float _height = isBigBulb ? 5 : 3;

                Vector3 _force = new Vector3(Random.Range(-_range, _range), Random.Range(.25f, _height), Random.Range(_range, _range));
                Instantiate(_items[i], transform.position, transform.rotation).GetComponent<PAF_Fruit>().AddForce(_force, m_lastHitPlayer);
            }
        }

        bulbAnimator.SetTrigger(explode_Hash);
    }
    
    public void ExplodeWithoutBool() => Explode(true);

    public void SetCanHit(bool _state)
    {
        if (isBigBulb)
            return;

        canHit = _state;
    }

    public void DestroyBulb()
    {
        if (isBigBulb)
            PAF_BulbManager.Instance.OnBigBulbExplode();

        soundSource.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetBulbExploding());
    }
}
