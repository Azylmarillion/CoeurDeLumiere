using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_Item : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    public ItemType CurrentItemType { get { return itemType; } }
    [SerializeField] SphereCollider colliderTrigger = null;
    [SerializeField] Rigidbody itemPhysics = null;
    [SerializeField] Collider spawnZone = null;
    [SerializeField] CDL_Player playerGrabbing = null;
    [SerializeField] bool canStun = false;
    [SerializeField, Range(0, 5)] float stunTime = .5f;
    Coroutine delayHitCoroutine = null;

    public bool IsReady => colliderTrigger && itemPhysics && spawnZone;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (playerGrabbing)
        {
            transform.position = playerGrabbing.transform.position;
            transform.rotation = playerGrabbing.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CDL_Player _pl = other.GetComponent<CDL_Player>();
        if (_pl && !canStun) _pl.SetItemNearby(this, true);
        else if (_pl && canStun)
        {
            _pl.Stun(stunTime);
            if (delayHitCoroutine != null)
            {
                StopCoroutine(delayHitCoroutine);
                delayHitCoroutine = null;
            }
            canStun = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CDL_Player _pl = other.GetComponent<CDL_Player>();
        if (_pl) _pl.SetItemNearby(this, false);
    }


    void Init()
    {
        if (!IsReady) return;
        Vector3 _spawnPos = Vector3.zero;
        float _minX = (spawnZone.transform.position.x - spawnZone.transform.localScale.x * spawnZone.bounds.size.x * 0.5f) / 2;
        float _minZ = (spawnZone.transform.position.z - spawnZone.transform.localScale.z * spawnZone.bounds.size.z * 0.5f) / 2;
        _spawnPos = new Vector3(Random.Range(_minX, -_minX), transform.position.y, Random.Range(_minZ, -_minZ));
        transform.position = _spawnPos;
    }

    public void SetGrabbed(bool _state, CDL_Player _player = null)
    {
        if (!IsReady) return;
        if (_state && _player)
        {
            itemPhysics.isKinematic = true;
            itemPhysics.useGravity = false;
            playerGrabbing = _player;
        }
        else
        {
            itemPhysics.isKinematic = false;
            itemPhysics.useGravity = true;
            itemPhysics.velocity = Vector3.zero;
            playerGrabbing = null;
        }
    }

    public void Throw()
    {
        if (!IsReady) return;
        itemPhysics.isKinematic = false;
        itemPhysics.useGravity = true;
        itemPhysics.velocity = Vector3.zero;
        playerGrabbing = null;
        itemPhysics.AddForce(transform.forward * 2000);
        delayHitCoroutine = StartCoroutine(DelayHit());
    }

    IEnumerator DelayHit()
    {
        canStun = true;
        yield return new WaitForSeconds(1.5f);
        canStun = false;
    }
}

public enum ItemType
{
    Fish,
    Bone,
    Nem,
    Sushi,
    Ramen,
    Chicken,
    Cake,
    Cookie
}
