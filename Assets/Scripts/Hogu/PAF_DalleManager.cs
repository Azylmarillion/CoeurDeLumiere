using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PAF_DalleManager : MonoBehaviour
{
    public static PAF_DalleManager I { get; private set; }

    [SerializeField] List<PAF_Dalle> allDallesToFall = new List<PAF_Dalle>();
    [SerializeField] List<PAF_Dalle> allRespawnableDalles = new List<PAF_Dalle>();
    public List<PAF_Dalle> AllRespawnableDalles { get { return allRespawnableDalles; } }
    [SerializeField] Transform center = null;
    [SerializeField] float fallDelay = .25f;
    [SerializeField, Range(0,.1f)] float shift = .05f;

    [SerializeField] Material[] dalleMaterials = new Material[] { };
        public Material[] DalleMaterials { get { return dalleMaterials; } }
    public float Shift { get { return shift; } }

    public bool IsReady => center && allDallesToFall.Count > 0;


    private void Awake()
    {
        I = this;
    }


    private void Start()
    {
        InitDalle();
        //StartFalling();
    }


    void InitDalle()
    {
        foreach (PAF_Dalle _dalle in allDallesToFall)
        {
            _dalle.gameObject.AddComponent<AudioSource>();
        }
    }

    public void StartFalling()
    {
        if (!IsReady) return;
        //allDallesToFall = allDallesToFall.OrderByDescending(d => Vector3.Distance(d.transform.position, center.position)).ToList(); // EXTERIEUR VERS INTERIEUR
        allDallesToFall = allDallesToFall.OrderByDescending(i => Guid.NewGuid()).ToList(); // RANDOM
        StartCoroutine(DelayFall());
    }
    
    IEnumerator DelayFall()
    {
        for (int i = 0; i < allDallesToFall.Count; i++)
        {
            allDallesToFall[i].Fall();
            yield return new WaitForSeconds(fallDelay);
        }
    }
}
