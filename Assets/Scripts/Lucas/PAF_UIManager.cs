using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class PAF_UIManager : MonoBehaviour
{
    /* UI :
     * 
     *  On pourrait faire tout le jeu dans un seul niveau :
     * on met tout le LD en enfant d'un GameObject,
     * et lorsqu'on recommence une partie, on détruit juste
     * tous les objets enfants du-dit GameObject, et on instancie
     * le prefab de "base" du niveau.
     * 
     * Gérer l'UI via UnityEvent & Animations
     * 
     * --> UIManager ?
     * 
    */

    #region Fields / Properties
    [Header("Animators")]
    /// <summary>
    /// Screen UI animator, making all different menus and sub-menus transitions.
    /// </summary>
    [SerializeField] private Animator screenAnimator = null;

    [Header("Texts")]
    /// <summary>
    /// Text used to display first player's score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI playerOneScore = null;
    [SerializeField] private TextMeshProUGUI playerOneReadyText = null;

    /// <summary>
    /// Text used to display second player's score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI playerTwoScore = null;
    [SerializeField] private TextMeshProUGUI playerTwoReadyText = null;

    [Header("Images")]
    [SerializeField] private Image m_playerOneReadyImage = null; 
    [SerializeField] private Image m_playerTwoReadyImage = null; 

    [Header("Parents")]
    /// <summary>
    /// Parent Object of the main menu 
    /// </summary>
    [SerializeField] private GameObject m_mainMenuObject = null;
    [SerializeField] private GameObject m_optionMenuParent = null;

    [Header("Volume")]
    [SerializeField] private UnityEngine.Audio.AudioMixer m_audioMixer = null; 
    [SerializeField] private float m_volumeIntensity = 1.0f; 
    public float VolumeIntensity
    {
        get
        {
            return m_volumeIntensity; 
        }
        set
        {
            m_volumeIntensity = value;
            PlayerPrefs.SetFloat("VolumeAttenuation", value);
            if (m_audioMixer) m_audioMixer.SetFloat("VolumeAttenuation", value); 
        }

    }
    [SerializeField] private Slider m_volumeSlider = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Updates the score of a player.
    /// </summary>
    /// <param name="_score">New player score.</param>
    /// <param name="_isPlayerOne">Is it the score of the first or second player ?</param>
    public void SetPlayerScore(bool _isPlayerOne, int _score)
    {
        if (_isPlayerOne)
        {
            playerOneScore.text = _score.ToString();
            //worldAnimator.SetTrigger("Score P1");
            screenAnimator.SetTrigger("P1 Score");
            return;
        }
        playerTwoScore.text = _score.ToString();
        //worldAnimator.SetTrigger("Score P2");
        screenAnimator.SetTrigger("P2 Score");
    }

    public void SetPlayerReady(bool _isPlayerOne)
    {
        if(_isPlayerOne)
        {
            m_playerOneReadyImage.color = Color.green; 
            playerOneReadyText.text = "Waiting for other player!"; 
            screenAnimator.SetTrigger("P1 Ready");
            return;
        }
        screenAnimator.SetTrigger("P2 Ready");
        playerTwoReadyText.text = "Waiting for other player!";
        m_playerTwoReadyImage.color = Color.green;
    }

    /// <summary>
    /// Set game UI for a given state.
    /// </summary>
    /// <param name="_state">New UI state.</param>
    public void SetUIState(UIState _state)
    {
        SetUIState((int)_state);
    }

    /// <summary>
    /// Set game UI for a given state.
    /// </summary>
    /// <param name="_state">New UI state.</param>
    public void SetUIState(int _state)
    {
        screenAnimator.SetInteger("State", _state);
    }

    /// <summary>
    /// Hide the main menu object if it exists
    /// </summary>
    private void HideMainMenu()
    {
        if (!m_mainMenuObject) return;
        m_mainMenuObject.SetActive(false); 
    }

    /// <summary>
    /// Init the audio mixer
    /// Get or set the value in the player prefs
    /// </summary>
    private void InitAudioMixer()
    {
        if (m_audioMixer)
        {
            float _value = 0;
            if (!PlayerPrefs.HasKey("VolumeAttenuation"))
            {
                m_audioMixer.GetFloat("VolumeAttenuation", out _value);
                PlayerPrefs.SetFloat("VolumeAttenuation", _value);
            }
            else
            {
                m_audioMixer.SetFloat("VolumeAttenuation", PlayerPrefs.GetFloat("VolumeAttenuation"));
                m_audioMixer.GetFloat("VolumeAttenuation", out _value);
            }
            if(m_volumeSlider)
            {
                m_volumeSlider.value = _value; 
            }
        }
    }

    /// <summary>
    /// Hide or show the option Menu
    /// </summary>
    private void DisplayOptionsMenu()
    {
        if (m_optionMenuParent == null) return;
        m_optionMenuParent.SetActive(!m_optionMenuParent.activeInHierarchy); 
    }

    /// <summary>
    /// Start the countdown animation 
    /// </summary>
    private void StartCountDown()
    {
        if(screenAnimator)
        {
            screenAnimator.SetTrigger("StartCountDown"); 
            return; 
        }
        PAF_GameManager.Instance.StartGame(); 
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        PAF_GameManager.OnEndCinematic += HideMainMenu;
        PAF_GameManager.OnEndCinematic += StartCountDown; 
        PAF_GameManager.OnPlayerScored += SetPlayerScore;
        PAF_GameManager.OnPlayerReady += SetPlayerReady; 
    }

    private void Start()
    {
        InitAudioMixer(); 
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DisplayOptionsMenu(); 
        }
    }

    private void OnDestroy()
    {
        PAF_GameManager.OnEndCinematic -= HideMainMenu;
        PAF_GameManager.OnEndCinematic -= StartCountDown;
        PAF_GameManager.OnPlayerScored -= SetPlayerScore;
        PAF_GameManager.OnPlayerReady -= SetPlayerReady;
    }
    #endregion

    #endregion
}
