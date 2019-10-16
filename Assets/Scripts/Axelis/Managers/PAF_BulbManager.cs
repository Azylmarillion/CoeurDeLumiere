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
        m_spawnedBulbs.Where(b => b.transform.position == m_centerPosition).FirstOrDefault()?.Explode(0); 

    }

    public void SelectFirstBulb()
    {
        if (m_checkBulbsCoroutine != null) return;  
        m_usedBulbsPosition = new Vector3[1] { m_centerPosition };
        m_checkBulbsCoroutine = StartCoroutine(WaitBulbsDestruction()); 
    }

    private void SelectNewBulbs()
    {

        if(m_checkBulbsCoroutine != null)
        {
            StopCoroutine(m_checkBulbsCoroutine);
            m_checkBulbsCoroutine = null; 
        }
        m_checkBulbsCoroutine = StartCoroutine(WaitBulbsDestruction());

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
