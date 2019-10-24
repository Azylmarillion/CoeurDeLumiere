using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 

public class PAF_BulbManager : MonoBehaviour 
{
    /* PAF_BulbManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    public static PAF_BulbManager Instance = null;

    [SerializeField]
    private Vector3[] m_bulbsPositions = new Vector3[] { };
    private Vector3[] m_usedBulbsPosition = new Vector3[] { };

    [SerializeField]
    private Vector3 m_centerPosition = Vector3.zero; 

    [SerializeField]
    private int m_bulbLimit = 1;
    [SerializeField]
    private float m_bulbDelay = 1.0f;

    [SerializeField]
    private PAF_Bulb m_bulbPrefab = null;

    private Coroutine m_checkBulbsCoroutine = null;
    private List<PAF_Bulb> m_spawnedBulbs = new List<PAF_Bulb>(); 
    #endregion

    #region Methods

    #region Original Methods

    #region IEnumerator
    private IEnumerator WaitBulbsDestruction()
    {
        if(!m_bulbPrefab)
        {
            m_checkBulbsCoroutine = null; 
            yield break; 
        }
        PAF_Bulb _bulb = null; 
        m_spawnedBulbs = new List<PAF_Bulb>();
        for (int i = 0; i < m_usedBulbsPosition.Length; i++)
        {
            _bulb = Instantiate(m_bulbPrefab, m_usedBulbsPosition[i], Quaternion.identity).GetComponent<PAF_Bulb>();
            m_spawnedBulbs.Add(_bulb);
            yield return new WaitForSeconds(Random.Range(.1f, .5f)); 
        }
        while (m_spawnedBulbs.Any(b => b != null))
        {
            yield return new WaitForSeconds(Mathf.Clamp(m_bulbDelay, .1f, 1.0f)); 
        }
        yield return new WaitForSeconds(m_bulbDelay);
        m_spawnedBulbs.Clear(); 
        SelectNewBulbs();
        m_checkBulbsCoroutine = null; 
    }
    #endregion 

    #region void
    public void CallBigBulb()
    {
        PAF_Bulb _centerBulb = m_spawnedBulbs.Where(b => b.transform.position == m_centerPosition).FirstOrDefault();
        if (_centerBulb)
        {
            _centerBulb.Explode(0);
            m_spawnedBulbs.Remove(_centerBulb); 
        }
        Instantiate(m_bulbPrefab, m_centerPosition, Quaternion.identity).GetComponent<PAF_Bulb>().SetBigBulb(); 
    }

    public void SelectFirstBulb()
    {
        if (m_checkBulbsCoroutine != null) return;  
        m_usedBulbsPosition = new Vector3[1] { m_centerPosition };
        m_checkBulbsCoroutine = StartCoroutine(WaitBulbsDestruction()); 
    }

    private void SelectNewBulbs()
    {
        m_usedBulbsPosition = GetRandomBulbs(); 

        if(m_checkBulbsCoroutine != null)
        {
            StopCoroutine(m_checkBulbsCoroutine);
            m_checkBulbsCoroutine = null; 
        }
        m_checkBulbsCoroutine = StartCoroutine(WaitBulbsDestruction());

    }

    public void DestroyAllBulbs() => m_spawnedBulbs.ForEach(b => b.Explode(0)); 
    #endregion

    #region Vector3
    private Vector3[] GetRandomBulbs()
    {
        List<Vector3> _availablesPositions = m_bulbsPositions.ToList();
        for (int i = 0; i < _availablesPositions.Count; i++)
        {
            if (m_usedBulbsPosition.ToList().Any(b => b == _availablesPositions[i]))
            {
                _availablesPositions.RemoveAt(i);
                i--;
            }
        }
        int _index = 0;
        List<Vector3> _positions = new List<Vector3>();
        if(_availablesPositions.Count < m_bulbLimit)
        {
            m_bulbLimit = _availablesPositions.Count; 
        }
        for (int i = 0; i < m_bulbLimit; i++)
        {
            _index = (int)Random.Range((int)0, (int)_availablesPositions.Count);
            _positions.Add(_availablesPositions[_index]);
            _availablesPositions.RemoveAt(_index);
        }
        return _positions.ToArray();
    }
    #endregion

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
