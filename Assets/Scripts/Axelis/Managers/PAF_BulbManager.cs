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

    [SerializeField] private PAF_Bulb[] m_bulbs = new PAF_Bulb[] { };
    private PAF_Bulb m_lastBulb = null; 
    #endregion

    #region Methods

    #region Original Methods
    public void CallBigBulb()
    {
        Debug.Log("Call super bulb!"); 
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
    #endregion

    #endregion
}
