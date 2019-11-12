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
    public bool Fell { get; private set; } = false;


    private void Start()
    {
        if (isShifting) transform.position += Vector3.up * Random.Range(-PAF_DalleManager.I.Shift * 2, -PAF_DalleManager.I.Shift);
        if (!PAF_DalleManager.I || PAF_DalleManager.I.DalleMaterials.Length <= 0 ||!randomColor) return;
        if (!renderer) renderer = GetComponent<Renderer>();
        if (renderer) renderer.material = PAF_DalleManager.I.DalleMaterials[Random.Range(0, PAF_DalleManager.I.DalleMaterials.Length)];
    }


    public void Fall()
    {
        if (!collider) collider = GetComponent<MeshCollider>();
        if (!renderer) renderer = GetComponent<Renderer>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!collider || !audioSource || ! renderer) return;
        renderer.enabled = false;
        collider.enabled = false;
        AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetDalleFalling();
        if (_clip) audioSource.PlayOneShot(_clip, .7f);
        Fell = true;
    }



}
