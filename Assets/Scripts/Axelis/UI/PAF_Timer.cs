using UnityEngine;
using TMPro; 

public class PAF_Timer : MonoBehaviour 
{
    /* PAF_Timer :
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
    [SerializeField] private Animator m_animator    = null;
    [SerializeField] private TMP_Text m_unitText    = null;
    [SerializeField] private TMP_Text m_tenText     = null;
    [SerializeField] private TMP_Text m_hundredText = null;

    private string m_displayedText = string.Empty; 
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Change the displayedText and call the animation according to the modulo 
    /// </summary>
    /// <param name="_remainingTime"></param>
    private void IncreaseTime(int _remainingTime)
    {
        m_displayedText = _remainingTime.ToString(); 
        if (!m_animator) return; 
        if((_remainingTime+1) % 100 == 0)
        {
            m_animator.SetTrigger("Hundred"); 
            return; 
        }
        if((_remainingTime+1) % 10 == 0)
        {
            m_animator.SetTrigger("Ten"); 
            return; 
        }
        m_animator.SetTrigger("Unit"); 
    }

    /// <summary>
    /// Apply the next text displayed on the timer
    /// </summary>
    public void ApplyNewText()
    {
        if (m_unitText) m_unitText.text = m_displayedText.Length < 1 ? "0" : m_displayedText[m_displayedText.Length - 1].ToString();
        if(m_tenText) m_tenText.text = m_displayedText.Length < 2 ? "0" : m_displayedText[m_displayedText.Length - 2].ToString();
        if (m_hundredText) m_hundredText.text = m_displayedText.Length < 3 ? "0" : m_displayedText[m_displayedText.Length - 3].ToString();
    }
	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    private void Awake()
    {
        PAF_GameManager.OnNextSecond += IncreaseTime; 
    }
	#endregion

	#endregion
}
