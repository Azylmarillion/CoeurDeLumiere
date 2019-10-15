using UnityEngine;

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
    private int m_bulbLimit = 1;
    [SerializeField]
    private float m_bulbDelay = 1.0f;

    private int m_index = 0; 
    #endregion

    #region Methods

    #region Original Methods
    public void CallBigBulb()
    {
        Debug.Log("Call super bulb!"); 
    }

    public void SelectNewBulbs()
    {
        if(m_index == 0)
        {
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < m_bulbsPositions.Length; i++)
        {
            Gizmos.DrawSphere(m_bulbsPositions[i], 1.0f); 
        }
    }
    #endregion

    #endregion
}
