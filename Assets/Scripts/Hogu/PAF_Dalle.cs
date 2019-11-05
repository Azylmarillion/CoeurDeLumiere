using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Dalle : MonoBehaviour
{
    [SerializeField] MeshCollider collider = null;
    [SerializeField] Renderer renderer = null;
    [SerializeField, Range(1,10)] float speedDecay = 2;
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] bool isShifting = true;
    [SerializeField] bool randomColor = true;
    [SerializeField] Material[] materials = new Material[] { };
    bool isFalling = false;
    bool isLeft = false;
    public bool Fell { get; private set; } = false;


    private void Start()
    {
        if (isShifting) transform.position += Vector3.up * Random.Range(-PAF_DalleManager.I.Shift * 2, -PAF_DalleManager.I.Shift);
        if (materials.Length <= 0 ||!randomColor) return;
        if (!renderer) renderer = GetComponent<Renderer>();
        if (renderer) renderer.material = materials[Random.Range(0, materials.Length)];
    }


    public void Fall(bool _isLeft)
    {
        if (!collider) collider = GetComponent<MeshCollider>();
        if (!renderer) renderer = GetComponent<Renderer>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!collider || !audioSource || ! renderer) return;
        renderer.enabled = false;
        collider.enabled = false;
        AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetDalleFalling();
        if (_clip) audioSource.PlayOneShot(_clip);
        isFalling = true;
        isLeft = _isLeft;
        Fell = true;
    }



}
