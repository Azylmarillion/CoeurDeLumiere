using UnityEngine;
using TMPro; 

public class PAF_Timer : MonoBehaviour 
{
    #region Fields / Properties
    [SerializeField] private Animator m_animator    = null;
    [SerializeField] private TMP_Text m_unitText    = null;
    [SerializeField] private TMP_Text m_tenText     = null;
    [SerializeField] private TMP_Text m_hundredText = null;

    private string m_displayedText = string.Empty;

    private readonly int hundred_Hash = Animator.StringToHash("Hundred");
    private readonly int ten_Hash = Animator.StringToHash("Ten");
    private readonly int unit_Hash = Animator.StringToHash("Unit");
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
        if((_remainingTime+1) % 100 == 0)
        {
            m_animator.SetTrigger(hundred_Hash); 
            return; 
        }
        if((_remainingTime+1) % 10 == 0)
        {
            m_animator.SetTrigger(ten_Hash); 
            return; 
        }
        m_animator.SetTrigger(unit_Hash); 
    }

    /// <summary>
    /// Apply the next text displayed on the timer
    /// </summary>
    public void ApplyNewText()
    {
        m_unitText.text = m_displayedText.Length < 1 ? "0" : m_displayedText[m_displayedText.Length - 1].ToString();
        m_tenText.text = m_displayedText.Length < 2 ? "0" : m_displayedText[m_displayedText.Length - 2].ToString();
        m_hundredText.text = m_displayedText.Length < 3 ? "0" : m_displayedText[m_displayedText.Length - 3].ToString();
    }
	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    private void Awake()
    {
        PAF_GameManager.OnNextSecond += IncreaseTime; 
    }

    private void OnDestroy()
    {
        PAF_GameManager.OnNextSecond -= IncreaseTime;
    }
    #endregion

    #endregion
}
