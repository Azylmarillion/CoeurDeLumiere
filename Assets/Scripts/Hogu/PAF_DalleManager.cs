using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Random = UnityEngine.Random;

public class PAF_DalleManager : MonoBehaviour
{
    private class GameObjectComparer : IComparer<GameObject>
    {
        public static readonly GameObjectComparer Default = new GameObjectComparer();

        public int Compare(GameObject _a, GameObject _b) => Random.Range(0f, .9f).CompareTo(Random.Range(0f, .9f));
    }

    private class DalleComparer : IComparer<PAF_Dalle>
    {
        public static readonly DalleComparer Default = new DalleComparer();

        public int Compare(PAF_Dalle _a, PAF_Dalle _b) => Vector3.Distance(_a.transform.position, I.Center).CompareTo(Vector3.Distance(_b.transform.position, I.Center));
    }

    public static PAF_DalleManager I { get; private set; }

    [SerializeField] PAF_Dalle[] allRespawnableDalles = new PAF_Dalle[] { };
    [SerializeField] GameObject[] allDallesGroups = new GameObject[]{ };
    public PAF_Dalle[] AllRespawnableDalles { get { return allRespawnableDalles; } }
    [SerializeField] Transform center = null;
    public Vector3 Center => center.position;

    [SerializeField] float fallDelay = .25f;
    [SerializeField, Range(0,.1f)] float shift = .05f;

    [SerializeField] GameObject dalleFX = null;

    [SerializeField] Material[] dalleMaterials = new Material[] { };
    public Material[] DalleMaterials { get { return dalleMaterials; } }
    public float Shift { get { return shift; } }

    public bool IsReady => center && allDallesGroups.Length > 0;

    [SerializeField, Range(0.0f, 1.0f)] private float m_screenshakeDuration = .1f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_screenshakeForce = .1f;

    private void Awake() => I = this;

    private void Start() => cameraPosition = PAF_GameManager.Instance.Camera.transform.position;

    private bool hasStarted = false;

    private bool isFalling = false;
    private bool isShaking = false;
    private float timerVar = 0;

    private Vector3 cameraPosition = new Vector3();

    private int groupIndex = 0;
    private int fallIndex = 0;

    PAF_Dalle[] fallingGroup = null;

    private void Update()
    {
        if (!hasStarted)
            return;

        if (isFalling)
        {
            if (isShaking)
            {
                PAF_GameManager.Instance.Camera.transform.position = cameraPosition + (Vector3)(Random.insideUnitCircle * m_screenshakeForce);

                timerVar += Time.deltaTime;
                if (timerVar >= m_screenshakeDuration)
                {
                    isShaking = false;
                    timerVar = 0;

                    PAF_GameManager.Instance.Camera.transform.position = cameraPosition;

                    fallingGroup = allDallesGroups[groupIndex].GetComponentsInChildren<PAF_Dalle>();
                    Array.Sort(fallingGroup, DalleComparer.Default);

                    allDallesGroups[groupIndex].GetComponent<AudioSource>().PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetDalleFalling(), .7f);
                }
            }
            else
            {
                // CHUTE AU FUR ET A MESURE
                timerVar += Time.deltaTime;
                if (timerVar >= .05f)
                {
                    fallingGroup[fallIndex].Fall();
                    Instantiate(dalleFX, fallingGroup[fallIndex].transform.position, fallingGroup[fallIndex].transform.rotation);

                    timerVar = 0;
                    fallIndex++;

                    if (fallIndex == fallingGroup.Length)
                    {
                        isFalling = false;
                        fallIndex = 0;
                        groupIndex++;
                    }
                }
            }
        }
        else if (groupIndex < allDallesGroups.Length)
        {
            timerVar += Time.deltaTime;
            if (timerVar >= fallDelay)
            {
                isFalling = true;
                isShaking = true;
                timerVar = 0;
            }
        }
    }

    public void StartFalling()
    {
        if (!IsReady)
            return;

        hasStarted = true;
        isFalling = true;
        isShaking = true;
        timerVar = 0;

        Array.Sort(allDallesGroups, GameObjectComparer.Default);
    }

    #if UNITY_EDITOR
    [MenuItem("CONTEXT/PAF_DalleManager/ReReroll Ground")]
    private static void ReRollGround(MenuCommand _command)
    {
        PAF_DalleManager I = FindObjectOfType<PAF_DalleManager>();
        PAF_Dalle[] _dalles = FindObjectsOfType<PAF_Dalle>();

        foreach (var _dalle in _dalles)
        {
            if (_dalle.IsShifting)
                _dalle.transform.position += Vector3.up * Random.Range(I.Shift * -2, -I.Shift);

            if (_dalle.RandomColor)
                _dalle.GetComponent<Renderer>().material = I.DalleMaterials[Random.Range(0, I.DalleMaterials.Length)];
        }
    }
    #endif
}
