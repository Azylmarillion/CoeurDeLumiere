using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_Item : MonoBehaviour
{
    [SerializeField] ItemType itemType;
        public ItemType CurrentItemType { get { return itemType; } }
    [SerializeField] Collider[] spawnZone = new Collider[] { };

    [SerializeField] Vector3 velocity = Vector3.zero;
    

    private void Start()
    {
        Init();
    }
    
    


    public void Kick()
    {
        Debug.Log(name);
    }

    public void Init()
    {
        if (spawnZone.Length < 1) return;
        int _rnd = Random.Range(0, spawnZone.Length);
        Vector3 _origin = spawnZone[_rnd].transform.position;
        Vector3 _range = spawnZone[_rnd].transform.localScale;
        Vector3 _randomRange = new Vector3(Random.Range(-_range.x, _range.x), transform.position.y, Random.Range(-_range.z, _range.z));
        transform.position = _origin + _randomRange;
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
