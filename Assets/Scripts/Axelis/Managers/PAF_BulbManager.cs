using UnityEngine;
using System.Collections.Generic;

public class PAF_BulbManager : MonoBehaviour 
{
    #region Fields / Properties
    public static PAF_BulbManager Instance = null;

    [SerializeField]
    private Vector3[] m_bulbsPositions = new Vector3[] { };

    [SerializeField]
    private Vector3 m_centerPosition = Vector3.zero; 

    [SerializeField]
    private int m_bulbLimit = 1;
    [SerializeField]
    private float m_bulbDelay = 1.0f;

    [SerializeField]
    private PAF_Bulb m_bulbPrefab = null;

    private List<PAF_Bulb> m_spawnedBulbs = new List<PAF_Bulb>();

    private bool m_bigBulbIsCalled = false;
    #endregion

    #region Methods

    #region Original Methods
    private bool doSpawn = false;
    private int spawnAmount = 0;
    private float spawnVar = 0;

    private readonly Vector3[] spawningPos = new Vector3[9];

    private bool isStarted = false;

    private void Update()
    {
        if (!isStarted)
            return;

        if (doSpawn)
        {
            spawnVar -= Time.deltaTime;
            if (spawnVar <= 0)
            {
                m_spawnedBulbs.Add(Instantiate(m_bulbPrefab, spawningPos[spawnAmount - 1], Quaternion.identity).GetComponent<PAF_Bulb>());

                spawnAmount--;
                if (spawnAmount == 0)
                {
                    doSpawn = false;
                    spawnVar = m_bulbDelay;
                }
                else
                {
                    spawnVar = Random.Range(.1f, .5f);
                }
            }
        }
        else if (m_spawnedBulbs.Count == 0)
        {
            spawnVar -= Time.deltaTime;
            if (spawnVar <= 0)
            {
                doSpawn = true;
                spawnVar = spawnVar = Random.Range(.1f, .5f);

                GetRandomBulbs();
            }
        }
    }

    public void SelectFirstBulb()
    {
        isStarted = true;
        doSpawn = true;
        spawnVar = spawnVar = Random.Range(.1f, .5f);

        spawnAmount = 1;
        spawningPos[0] = m_centerPosition;
    }

    public void CallBigBulb()
    {
        m_bigBulbIsCalled = true;

        DestroyAllBulbs();

        PAF_Bulb _bulb = Instantiate(m_bulbPrefab, m_centerPosition, Quaternion.identity).GetComponent<PAF_Bulb>();
        _bulb.SetBigBulb();

        m_spawnedBulbs.Add(_bulb);
    }

    public void OnBigBulbExplode() => m_bigBulbIsCalled = false;

    public void RemoveBulb(PAF_Bulb _bulb) => m_spawnedBulbs.Remove(_bulb);

    public void DestroyAllBulbs()
    {
        for (int _i = 0; _i < m_spawnedBulbs.Count; _i++)
            m_spawnedBulbs[_i].Explode(false);

        m_spawnedBulbs.Clear();
    }

    private void GetRandomBulbs()
    {
        spawnAmount = m_bulbLimit;
        for (int _i = 0; _i < spawnAmount; _i++)
        {
            int _index = Random.Range(0, m_bulbsPositions.Length);
            for (int _j = 0; _j < _i; _j++)
            {
                if (spawningPos[_j] == m_bulbsPositions[_index])
                {
                    _index = Random.Range(0, m_bulbsPositions.Length);
                    _j = 0;
                }
            }

            spawningPos[_i] = m_bulbsPositions[_index];
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return; 
        }

        Instance = this;
        PAF_Bulb.InitGoldenAmount();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < m_bulbsPositions.Length; i++)
        {
            Gizmos.DrawSphere(m_bulbsPositions[i], .5f); 
        }
    }
    #endregion

    #endregion
}
