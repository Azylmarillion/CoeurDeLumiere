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

        public static PAF_UIManager Instance { get; private set; }

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

    [SerializeField] private TextMeshProUGUI highScoreText = null;

    /// <summary>
    /// Text used to display second player's score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI playerTwoScore = null;
    [SerializeField] private TextMeshProUGUI playerTwoReadyText = null;

    [Header("Score")]
    [SerializeField] private GameObject playersScoreAnchor = null;
    [SerializeField] private TextMeshProUGUI playerOneTotalScore = null;
    [SerializeField] private Animator playerOneTotalScoreAnimator = null;
    [SerializeField] private TextMeshProUGUI playerTwoTotalScore = null;
    [SerializeField] private Animator playerTwoTotalScoreAnimator = null;

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

    [SerializeField] private float m_volumeIntensityMusic = 1.0f;
    public float VolumeIntensityMusic
    {
        get
        {
            return m_volumeIntensityMusic;
        }
        set
        {
            m_volumeIntensityMusic = value;
            PlayerPrefs.SetFloat("VolumeAttenuationMusic", value);
            if (m_audioMixer) m_audioMixer.SetFloat("VolumeAttenuationMusic", value);
        }

    }
    [SerializeField] private Slider m_volumeSliderMusic = null;

    [Header("Credits")]
    [SerializeField] private TextMeshProUGUI winnerScoreText = null;
    [SerializeField] private TextMeshProUGUI loserScoreText = null;

    [SerializeField] private GameObject equalityPresentator = null;
    [SerializeField] private GameObject winnerPresentator = null;

    [SerializeField] private GameObject newRecordFeedback = null;
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
        string _text = _score > 0 ? "+" : string.Empty;
        if (_isPlayerOne)
        {
            playerOneTotalScore.text = (int.Parse(playerOneTotalScore.text) + _score).ToString();
            playerOneTotalScoreAnimator.SetTrigger("Score");
            playerOneScore.text = _text + _score.ToString();
            //worldAnimator.SetTrigger("Score P1");
            screenAnimator.SetTrigger("P1 Score");
            return;
        }
        playerTwoTotalScore.text = (int.Parse(playerTwoTotalScore.text) + _score).ToString();
        playerTwoTotalScoreAnimator.SetTrigger("Score");
        playerTwoScore.text = _text + _score.ToString();
        //worldAnimator.SetTrigger("Score P2");
        screenAnimator.SetTrigger("P2 Score");
    }

    public void SetPlayerReady(bool _isPlayerOne)
    {
        if(_isPlayerOne)
        {
            m_playerOneReadyImage.color = Color.green; 
            playerOneReadyText.text = "En attente de l'autre Joueur !"; 
            screenAnimator.SetTrigger("P1 Ready");
            return;
        }
        screenAnimator.SetTrigger("P2 Ready");
        playerTwoReadyText.text = "En attente de l'autre Joueur !";
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
    public void HideMainMenu()
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
            if (!PlayerPrefs.HasKey("VolumeAttenuationMusic"))
            {
                m_audioMixer.GetFloat("VolumeAttenuationMusic", out _value);
                PlayerPrefs.SetFloat("VolumeAttenuationMusic", _value);
            }
            else
            {
                m_audioMixer.SetFloat("VolumeAttenuationMusic", PlayerPrefs.GetFloat("VolumeAttenuationMusic"));
                m_audioMixer.GetFloat("VolumeAttenuationMusic", out _value);
            }
            if (m_volumeSliderMusic)
            {
                m_volumeSliderMusic.value = _value;
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

        #if !UNITY_EDITOR
        // Hide cursor
        Cursor.visible = m_optionMenuParent.activeInHierarchy;
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        #endif
    }

    /// <summary>
    /// Start the countdown animation 
    /// </summary>
    private void StartCountDown()
    {
        if(screenAnimator)
        {
            playersScoreAnchor.SetActive(true);

            screenAnimator.SetTrigger("StartCountDown"); 
            return; 
        }
        PAF_GameManager.Instance.StartGame(); 
    }

    /// <summary>
    /// Displays the end game menu.
    /// </summary>
    /// <param name="_bestPlayer">Best player : 0 for equality, 1 for first player and other for second one.</param>
    /// <param name="_highScore">Highscore of this game.</param>
    public void DisplayEndMenu(int _bestPlayer, int _highScore, int _otherScore = 0)
    {
        string _winnerText = string.Empty;

        if (_bestPlayer == 0)
        {
            //Egalité
            _winnerText = "Egalite : ";
            equalityPresentator.SetActive(true);
            loserScoreText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            string _loserText = string.Empty;

            if (_bestPlayer == 1)
            {
                // P1 Gagne
                _winnerText = "ROUGE";
                _loserText = "BLEU ";
            }
            else
            {
                // P2 Gagne
                _winnerText = "BLEU";
                _loserText = "ROUGE ";
            }
            _winnerText += " Gagne : ";
            winnerPresentator.SetActive(true);

            loserScoreText.text = _loserText + _otherScore;
        }

        winnerScoreText.text = _winnerText + _highScore;
        
        if (_highScore >= PAF_GameManager.HighScore)
        {
            newRecordFeedback.SetActive(true);
        }
    }
#endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        Instance = this;
        PAF_GameManager.OnEndCinematic += HideMainMenu;
        PAF_GameManager.OnEndCinematic += StartCountDown; 
        PAF_GameManager.OnPlayerScored += SetPlayerScore;
        PAF_GameManager.OnPlayerReady += SetPlayerReady;
    }

    private void Start()
    {
        InitAudioMixer();
        if (highScoreText) highScoreText.text = "Meilleur Score : " + (PAF_GameManager.HighScore > 0 ? PAF_GameManager.HighScore.ToString() : "-");
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
