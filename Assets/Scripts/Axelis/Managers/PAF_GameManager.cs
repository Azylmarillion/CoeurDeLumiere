using System;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PAF_GameManager : MonoBehaviour 
{
    #region Events
    /// <summary>
    /// Event called during every second between the start and the end of the game.
    /// The <see cref="int"/> argument is the remaining Time. 
    /// </summary>
    public static event Action<int> OnNextSecond = null; 
    #endregion

    #region Fields / Properties
    public static PAF_GameManager Instance = null;

    [SerializeField] private new Camera camera = null;
    public Camera Camera => camera;

    [SerializeField]private bool m_playerOneIsReady = false;
    [SerializeField]private bool m_playerTwoIsReady = false;
    private bool m_gameIsReadyToStart = false;
    private bool m_gameIsOver = false; 
    public bool PlayersAreReadyToStart { get { return m_playerOneIsReady && m_playerTwoIsReady;  } }
    public bool GameIsReadyToStart { get { return m_playerOneIsReady && m_playerTwoIsReady && m_gameIsReadyToStart; } }

    /// <summary>
    /// Duration of the game
    /// </summary>
    [SerializeField, Range(1, 500)] private int m_gameDuration = 120;
    /// <summary>
    /// Current time spent during the game
    /// </summary>
    private int m_currentGameTime = 0;
    public float CurrentGameTimePercent { get { return m_currentGameTime / 120f; } }

    /// <summary>
    /// Highest score registered.
    /// </summary>
    private static int m_highScore = 0;
    public static int HighScore { get { return m_highScore; } }

    /// <summary>
    /// Score of player One
    /// </summary>
    private int m_playerOneScore = 0;
    /// <summary>
    /// Score of player Two
    /// </summary>
    private int m_playerTwoScore = 0;
    public bool PlayerOneIsLeading
    {
        get { return m_playerOneScore > m_playerTwoScore;  }
    }
    [SerializeField] private ParticleSystem playerOneConfettis = null;
    [SerializeField] private ParticleSystem playerTwoConfettis = null;

    [SerializeField] private PlayableDirector credits = null;

    private static string saveFileFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Pafztek"); } }

    private static string saveFileName = "save.sav";

    private static string saveFilePath { get { return Path.Combine(saveFileFolder, saveFileName); } }

    [Header("Tuto")]
    [SerializeField] private PAF_Fruit leftFruit = null;
    [SerializeField] private Vector3 leftFruitForce = Vector3.zero;
    [SerializeField] private PAF_Fruit rightFruit = null;
    [SerializeField] private Vector3 rightFruitForce = Vector3.zero;

    /// <summary>
    /// Array of all events called during the game at a certain timecode
    /// </summary>
    [Header("Game Events")]
    [SerializeField] private PAF_Event[] m_gameEvents = new PAF_Event[] { };

    [Header("Video")]
    [SerializeField] private UnityEngine.Video.VideoPlayer m_videoPlayer = null;
    [SerializeField] private GameObject m_videoRenderer = null;
    [SerializeField] private float m_speedFadeCinematic = 2;

    [Header("Datas")]
    [SerializeField] private PAF_SoundData m_soundDatas = null;
    [SerializeField] private PAF_VFXData m_vfxDatas = null;

    [SerializeField] private UnityEngine.Audio.AudioMixer audioMixer = null;
    [SerializeField] private AudioSource m_gameMusic = null;
    [SerializeField] private AudioSource m_audioSource = null;

    public PAF_SoundData SoundDatas
    {
        get { return m_soundDatas;  }
    }
    public PAF_VFXData VFXDatas
    {
        get
        {
            return m_vfxDatas; 
        }
    }

    public UnityEngine.Audio.AudioMixer AudioMixer { get { return audioMixer; } }
    #endregion

    #region Methods

    #region Original Methods
    private bool doTicTac = false;
    private float ticTacVar = 0;

    private float fadeCineVar = 0;

    private bool doIncreaseTime = false;
    private float increaseTimeVar = 0;

    private bool isPlayingVideo = false;
    private float playingVideoVar = 0;

    private int videoState = 0;

    private bool doHideMenu = false;
    private float hideMenuVar = 0;

    private float touchTimer = 0;

    private void Update()
    {
        // Video playing.
        if (isPlayingVideo)
        {
            if (playingVideoVar < 1)
            {
                playingVideoVar += Time.deltaTime;
            }
            else if (videoState < 2)
            {
                if (videoState == 0)
                {
                    videoState = 1;
                    m_videoPlayer.Prepare();
                }
                else if (m_videoPlayer.isPrepared)
                {
                    videoState = 2;
                    m_videoPlayer.Play();

                    doHideMenu = true;
                    hideMenuVar = 0;
                }
            }
            else if (m_videoPlayer.isPlaying)
            {
                #if UNITY_EDITOR
                if (Input.GetKey(KeyCode.Keypad1) && Input.GetKey(KeyCode.Keypad2) && Input.GetKey(KeyCode.Keypad3))
                {
                    m_videoPlayer.Stop();
                    m_gameMusic.time = 20;
                }
                #else
                if (Input.touchCount > 0)
                {
                    touchTimer += Time.deltaTime;
                }
                else if (touchTimer > 0)
                {
                    if (touchTimer < .1f)
                    {
                        m_videoPlayer.Stop();
                        m_gameMusic.time = 20;
                    }
                    touchTimer = 0;
                }
                #endif
            }
            else if (videoState == 2)
            {
                videoState = 3;

                HideCinematic();
                PAF_UIManager.Instance.HideMainMenu();
                PAF_UIManager.Instance.StartCountDown();
            }
            else
            {
                playingVideoVar += Time.deltaTime;
                if (playingVideoVar > 1.5f)
                {
                    isPlayingVideo = false;
                    TutoAnim();
                }
            }
        }

        // Hide Menu.
        if (doHideMenu)
        {
            hideMenuVar += Time.deltaTime;
            if (hideMenuVar >= .3f)
            {
                doHideMenu = false;

                m_gameMusic.Play();
                PAF_UIManager.Instance.HideMainMenu();
            }
        }

        // Time increase.
        if (doIncreaseTime)
        {
            increaseTimeVar += Time.deltaTime;
            if (increaseTimeVar >= 1)
            {
                increaseTimeVar = increaseTimeVar % 1;

                OnNextSecond.Invoke(m_gameDuration - m_currentGameTime);

                for (int _i = 0; _i < m_gameEvents.Length; _i++)
                {
                    if (!m_gameEvents[_i].HasBeenCalled && (m_gameEvents[_i].CallingTime <= m_currentGameTime))
                    {
                        m_gameEvents[_i].CallEvent();
                    }
                }

                m_currentGameTime++;
                if (m_currentGameTime == m_gameDuration)
                {
                    doIncreaseTime = false;
                    m_currentGameTime = 0;
                    EndGame();
                }
            }
        }

        // Tic tac.
        if (doTicTac)
        {
            ticTacVar += Time.deltaTime;
            if (ticTacVar >= 1)
            {
                ticTacVar = ticTacVar % 1;
                m_audioSource.PlayOneShot(SoundDatas.GetTicTic(), 3);
            }
        }
    }

    /// <summary>
    /// Start the IncreasePlayingTime Coroutine
    /// </summary>
    public void StartGame()
    {
        m_gameIsReadyToStart = true;

        doIncreaseTime = true;
        increaseTimeVar = 0;

        // Play sound.
        m_audioSource.PlayOneShot(SoundDatas.GetWhistle(), 1.0f);
    }

    /// <summary>
    /// Disable the cinematic if it exists
    /// </summary>
    void HideCinematic()
    {
        m_videoPlayer.Stop();
        m_videoPlayer.enabled = true;

        m_videoRenderer.SetActive(false);
        fadeCineVar = 0;
    }

    /// <summary>
    /// Loads highest score.
    /// </summary>
    private void LoadHighScore()
    {
        if (!Directory.Exists(saveFileFolder) || !File.Exists(saveFilePath)) return;
        int.TryParse(File.ReadAllText(saveFilePath).Trim(), out m_highScore);
    }

    /// <summary>
    /// Registers new game high score.
    /// </summary>
    /// <param name="_newHighScore">New high score.</param>
    public void RegisterHighScore(int _newHighScore)
    {
        m_highScore = _newHighScore;
        m_audioSource.PlayOneShot(SoundDatas.GetHighscoreSound(), .8f);

        if (!Directory.Exists(saveFileFolder)) Directory.CreateDirectory(saveFileFolder);
        File.WriteAllText(saveFilePath, _newHighScore.ToString());
    }

    /// <summary>
    /// Called at the end of the game.
    /// </summary>
    private void EndGame()
    {
        PAF_Player.Players[0].EndGame();
        PAF_Player.Players[1].EndGame();

        PAF_MainMenuMusic.Instance.StartMusic();

        doTicTac = false;

        // Play sound.
        m_audioSource.PlayOneShot(SoundDatas.GetWhistle(), 1.0f);

        int _highestScore = 0;

        if (m_playerOneScore == m_playerTwoScore)
        {
            _highestScore = m_playerOneScore;
            PAF_UIManager.Instance.DisplayEndMenu(0, _highestScore);
        }
        else if (m_playerOneScore > m_playerTwoScore)
        {
            _highestScore = m_playerOneScore;
            PAF_UIManager.Instance.DisplayEndMenu(1, _highestScore, m_playerTwoScore);
        }
        else
        {
            _highestScore = m_playerTwoScore;
            PAF_UIManager.Instance.DisplayEndMenu(2, _highestScore, m_playerOneScore);
        }

        if (_highestScore > m_highScore) RegisterHighScore(_highestScore);

        m_playerOneIsReady = false;
        m_playerTwoIsReady = false;
        m_gameIsOver = true;

        // Une avalanche de confettis
        playerOneConfettis.Play();
        playerTwoConfettis?.Play();

        credits.Play();
        credits.stopped += (PlayableDirector _a) => SceneManager.LoadScene(0);
    }

    private void TutoAnim()
    {
        leftFruit.AddForce(leftFruitForce);
        rightFruit.AddForce(rightFruitForce);
    }

    /// <summary>
    /// Increase the score of the player according to the given points 
    /// </summary>
    /// <param name="_isFirstPlayer">Is the rewarded player is player one?</param>
    /// <param name="_increase">The points to add to the player score</param>
    public void IncreasePlayerScore(bool _isFirstPlayer, int _increase)
    {
        m_audioSource.PlayOneShot(SoundDatas.GetCrowdCheer(), 1.5f);

        if (_isFirstPlayer)
        {
            m_playerOneScore += _increase;
            playerOneConfettis.Play();
        }
        else
        {
            m_playerTwoScore += _increase;
            playerTwoConfettis.Play();
        }

        PAF_Player.Players[0].CheckScore(_isFirstPlayer, _increase);
        PAF_Player.Players[1].CheckScore(_isFirstPlayer, _increase);

        PAF_UIManager.Instance.SetPlayerScore(_isFirstPlayer, _increase);
    }

    /// <summary>
    /// Set camera aspect to 16 / 9.
    /// </summary>
    public void SetCameraAspect(float _width, float _height)
    {
        // Calculates camera rect for aspect ratio of 16/9
        float _heightRatio = (_width / _height) / (16f / 9f);

        // If ratio is correct, the keep it
        if (_heightRatio == 1)
        {
            camera.rect = new Rect(0, 0, 1, 1);
        }
        // If ratio is inferior to one (not large enough), set black bars on top & bottom of the screen
        else if (_heightRatio < 1)
        {
            camera.rect = new Rect(0, (1 - _heightRatio) / 2, 1, _heightRatio);
        }
        // If superior to one (too large), then set black bars on left & right of the screen
        else
        {
            float _widthRatio = 1 / _heightRatio;
            camera.rect = new Rect((1 - _widthRatio) / 2, 0, _widthRatio, 1);
        }
    }

    /// <summary>
    /// Set if the player is ready
    /// If both players are ready, start the game
    /// </summary>
    /// <param name="_isPlayerOne"></param>
    public void SetPlayerReady(bool _isPlayerOne)
    {
        if (m_gameIsOver)
            return;

        m_audioSource.PlayOneShot(SoundDatas.GetSelectMenu(), .8f);

        if (_isPlayerOne)
        {
            m_playerOneIsReady = true;
        }
        else
        {
            m_playerTwoIsReady = true;
        }

        if (PlayersAreReadyToStart)
        {
            isPlayingVideo = true;
            playingVideoVar = 0;
            videoState = 0;

            PAF_MainMenuMusic.Instance.StopMusic();
        }
    }

    public void StartTicTicSound()
    {
        doTicTac = true;
        ticTacVar = 0;
    }
#endregion

#region Unity Methods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            LoadHighScore();
        }
        else
        {
            Destroy(this);
            return; 
        }

        SetCameraAspect(Screen.width, Screen.height);
    }

    private void Start()
    {
#if !UNITY_EDITOR
        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }
#endregion

#endregion
}
