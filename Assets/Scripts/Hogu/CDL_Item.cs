using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_Item : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    public ItemType CurrentItemType { get { return itemType; } }
    [SerializeField] SphereCollider colliderTrigger = null;
    [SerializeField] Rigidbody itemPhysics = null;
    [SerializeField] Collider[] spawnZone = new Collider[] { };
    [SerializeField] CDL_Player playerGrabbing = null;
    [SerializeField] bool canStun = false;
    [SerializeField, Range(0, 5)] float stunTime = .5f;
    [SerializeField] LayerMask wallLayer = 0;
    [SerializeField] LayerMask characterLayer = 0;
    [SerializeField] Collider moveArea = null;

    public bool IsReady => colliderTrigger && itemPhysics;

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
    }

    private void OnTriggerExit(Collider other)
    {
        CDL_Player _pl = other.GetComponent<CDL_Player>();
        if (_pl) _pl.SetItemNearby(this, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canStun = false;
        }
    }

    public void Init()
    {
        if (!IsReady) return;
        int _rnd = Random.Range(0, spawnZone.Length);

        Vector3 _origin = spawnZone[_rnd].transform.position;
        Vector3 _range = spawnZone[_rnd].transform.localScale;
        Vector3 _randomRange = new Vector3(Random.Range(-_range.x, _range.x), transform.position.y, Random.Range(-_range.z, _range.z));
        transform.position = _origin + _randomRange;
    }

    public void SetGrabbed(bool _state, CDL_Player _player = null)
    {
        if (!IsReady) return;
        if (_state && _player)
        {
            playerGrabbing = _player;
        }
        else
        {
            playerGrabbing = null;
        }
    }

    public void Throw()
    {
        if (!IsReady) return;
        itemPhysics.AddForce(playerGrabbing.transform.forward * 2000);
        playerGrabbing = null;
        InvokeRepeating("Thrown", 0, .01f);
        canStun = true;
    }

    void Thrown()
    {
        RaycastHit _hitPlayer;
        bool _hasHitPlayer = false;
        if ((_hasHitPlayer = Physics.Raycast(transform.position, transform.forward, out _hitPlayer, 1.5f, characterLayer)) || Physics.Raycast(transform.position, transform.forward, 1.5f, wallLayer))
        {
            if(_hasHitPlayer)
            {
                CDL_Player _pl = _hitPlayer.transform.GetComponent<CDL_Player>();
                if (_pl) _pl.Stun(stunTime);
            }
            canStun = false;
            CancelInvoke("Thrown");
        }
        Vector3 _nextPos = transform.position + transform.forward;
        if (moveArea.bounds.Contains(_nextPos)) transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * 25);
    }
}

public enum ItemType
{
    None,
    Fish,
    Bone,
    Nem,
    Sushi,
    Ramen,
    Chicken,
    Cake,
    Cookie
}
