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

    [SerializeField, Range(0.0f, 1.0f)] private float m_screenshakeDuration = .1f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_screenshakeForce = .1f;

    private void Awake()
    {
        I = this;
    }


    private void Start()
    {
        InitDalle();
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
        Vector3 _initialPosition = Camera.main.transform.position;
        float _timer = 0;
        Vector2 _offset = Vector2.zero;

        for (int i = 0; i < allDallesToFall.Count; i++)
        {
            while (_timer < m_screenshakeDuration)
            {
                _offset = UnityEngine.Random.insideUnitCircle * m_screenshakeForce;
                Camera.main.transform.position = _initialPosition + (Vector3)_offset;
                yield return new WaitForEndOfFrame();
                _timer += Time.deltaTime;
            }
            _timer = 0;
            Camera.main.transform.position = _initialPosition;
            allDallesToFall[i].Fall();
            yield return new WaitForSeconds(fallDelay);
        }
    }
}
