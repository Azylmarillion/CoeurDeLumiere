using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class PAF_UIManager : MonoBehaviour
{
    public static PAF_UIManager Instance { get; private set; }

    #region Fields / Properties
    [Header("Animators")]

    [SerializeField] private Animator p1ScoreAnim = null;
    [SerializeField] private Animator p2ScoreAnim = null;

    [SerializeField] private Animator p1ReadyAnim = null;
    [SerializeField] private Animator p2ReadyAnim = null;

    [SerializeField] private Animator countdownAnim = null;

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

    [Header("Controllers")]
    [SerializeField] private RectTransform controllerOneArea = null;
    [SerializeField] private RectTransform controllerOneJoystick = null;
    [SerializeField] private RectTransform controllerTwoArea = null;
    [SerializeField] private RectTransform controllerTwoJoystick = null;

    [Header("Score")]
    [SerializeField] private GameObject playersScoreAnchor = null;
    [SerializeField] private TextMeshProUGUI playerOneTotalScore = null;
    [SerializeField] private Animator playerOneTotalScoreAnimator = null;
    [SerializeField] private TextMeshProUGUI playerTwoTotalScore = null;
    [SerializeField] private Animator playerTwoTotalScoreAnimator = null;

    [Header("Images")]
    [SerializeField] private Image m_playerOneReadyImage = null;
    [SerializeField] private Button m_playerOneReadyButton = null;

    [SerializeField] private Image m_playerTwoReadyImage = null;
    [SerializeField] private Button m_playerTwoReadyButton = null;

    [Header("Parents")]

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
            m_audioMixer.SetFloat("VolumeAttenuation", value); 
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
            m_audioMixer.SetFloat("VolumeAttenuationMusic", value);
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

    private readonly int score_Hash = Animator.StringToHash("Score");
    private readonly int trigger_Hash = Animator.StringToHash("Trigger");

    private readonly int state_Hash = Animator.StringToHash("State");

    #region Methods

    #region Original Methods
    public void QuitGame() => Application.Quit();

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
            playerOneTotalScoreAnimator.SetTrigger(score_Hash);
            playerOneScore.text = _text + _score.ToString();
            p1ScoreAnim.SetTrigger(trigger_Hash);
        }
        else
        {
            playerTwoTotalScore.text = (int.Parse(playerTwoTotalScore.text) + _score).ToString();
            playerTwoTotalScoreAnimator.SetTrigger(score_Hash);
            playerTwoScore.text = _text + _score.ToString();
            p2ScoreAnim.SetTrigger(trigger_Hash);
        }
    }

    public void SetPlayerReady(bool _isPlayerOne)
    {
        if(_isPlayerOne)
        {
            m_playerOneReadyButton.interactable = false;
            m_playerOneReadyImage.color = Color.green; 
            playerOneReadyText.text = "Waiting for other player !"; 
            p1ReadyAnim.SetTrigger(trigger_Hash);
        }
        else
        {
            m_playerTwoReadyButton.interactable = false;
            p2ReadyAnim.SetTrigger(trigger_Hash);
            playerTwoReadyText.text = "Waiting for other player !";
            m_playerTwoReadyImage.color = Color.green;
        }

        PAF_GameManager.Instance.SetPlayerReady(_isPlayerOne);
    }

    /// <summary>
    /// Hide the main menu object if it exists
    /// </summary>
    public void HideMainMenu() => m_mainMenuObject.SetActive(false);

    /// <summary>
    /// Init the audio mixer
    /// Get or set the value in the player prefs
    /// </summary>
    private void InitAudioMixer()
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
        m_volumeSlider.value = _value;

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
        m_volumeSliderMusic.value = _value;
    }

    /// <summary>
    /// Hide or show the option Menu
    /// </summary>
    public void DisplayOptionsMenu()
    {
        m_optionMenuParent.SetActive(!m_optionMenuParent.activeInHierarchy);
        Time.timeScale = m_optionMenuParent.activeInHierarchy ? 0 : 1;

        #if !UNITY_EDITOR
        // Hide cursor
        Cursor.visible = m_optionMenuParent.activeInHierarchy;
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        #endif
    }

    /// <summary>
    /// Start the countdown animation 
    /// </summary>
    public void StartCountDown()
    {
        playersScoreAnchor.SetActive(true);
        countdownAnim.SetTrigger(trigger_Hash);
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
            _winnerText = "Draw : ";
            equalityPresentator.SetActive(true);
            loserScoreText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            string _loserText = string.Empty;

            if (_bestPlayer == 1)
            {
                // P1 Gagne
                _winnerText = "RED";
                _loserText = "BLUE : ";
            }
            else
            {
                // P2 Gagne
                _winnerText = "BLUE";
                _loserText = "RED : ";
            }
            _winnerText += " Wins : ";
            winnerPresentator.SetActive(true);

            loserScoreText.text = _loserText + _otherScore;
        }

        winnerScoreText.text = _winnerText + _highScore;
        
        if (_highScore >= PAF_GameManager.HighScore)
        {
            newRecordFeedback.SetActive(true);
        }
    }

    public RectTransform GetPlayerBounds(bool _isPlayerOne)
    {
        return _isPlayerOne ? controllerOneArea : controllerTwoArea;
    }

    public void UpdatePlayerJoystick (bool _isPlayerOne, Vector2 _position)
    {
        if (_isPlayerOne)
            controllerOneJoystick.localPosition = _position.normalized * controllerOneArea.sizeDelta / 4;
        else
            controllerTwoJoystick.localPosition = _position.normalized * controllerTwoArea.sizeDelta / 4;
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitAudioMixer();
        highScoreText.text = "High Score : " + (PAF_GameManager.HighScore > 0 ? PAF_GameManager.HighScore.ToString() : "-");
    }
    #endregion

    #endregion
}
