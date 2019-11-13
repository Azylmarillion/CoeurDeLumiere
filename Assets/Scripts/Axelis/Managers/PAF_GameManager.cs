using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class PAF_GameManager : MonoBehaviour 
{
    /* PAF_GameManager :
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
    public static event Action OnEndCinematic = null; 
    /// <summary>
    /// Event called when the game Starts
    /// </summary>
    public static event Action OnGameStart = null;
    /// <summary>
    /// Event called during every second between the start and the end of the game.
    /// The <see cref="int"/> argument is the remaining Time. 
    /// </summary>
    public static event Action<int> OnNextSecond = null; 
    /// <summary>
    /// Event called when the game Ends
    /// The argument is the score of both players
    /// </summary>
    public static event Action<int, int> OnGameEnd = null;
    /// <summary>
    /// Event called when one of the player scores
    /// </summary>
    public static event Action<bool, int> OnPlayerScored = null;
    /// <summary>
    /// Event called when one of the player is ready
    /// </summary>
    public static event Action<bool> OnPlayerReady = null; 
    #endregion

    #region Fields / Properties
    public static PAF_GameManager Instance = null;


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

    /// <summary>
    /// Array of all events called during the game at a certain timecode
    /// </summary>
    [Header("Game Events")]
    [SerializeField] private PAF_Event[] m_gameEvents = new PAF_Event[] { };

    [Header("Video")]
    [SerializeField] private UnityEngine.Video.VideoPlayer m_videoPlayer = null;
    [SerializeField] private Material m_introMaterial = null;
    [SerializeField] private float m_speedFadeCinematic = 2;
    [SerializeField] private PAF_SoundData m_soundDatas = null;
    [SerializeField] private PAF_VFXData m_vfxDatas = null;

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
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Start the video player to start the cinematic
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartVideoPlayer()
    {
        yield return new WaitForSeconds(1.0f); 
        if (m_videoPlayer && m_introMaterial)
        {
            m_introMaterial.color = new Color(m_introMaterial.color.r, m_introMaterial.color.g, m_introMaterial.color.b, 1);
            m_videoPlayer.Prepare();
            while(!m_videoPlayer.isPrepared)
            {
                yield return new WaitForSeconds(.1f);
            }
            PAF_UIManager.Instance?.HideMainMenu();
            m_videoPlayer.Play();
            while (m_videoPlayer.isPlaying)
            {
                if (Input.GetKey(KeyCode.Keypad1) && Input.GetKey(KeyCode.Keypad2) && Input.GetKey(KeyCode.Keypad3))
                {
                    m_videoPlayer.Stop();
                }
                yield return new WaitForSeconds(.1f);
            }
        }
        OnEndCinematic?.Invoke(); 
    }

    /// <summary>
    /// Increase playing time and call the events when the timer is greater than their calling time
    /// </summary>
    /// <returns></returns>
    private IEnumerator IncreasePlayingTime()
    {
        OnGameStart?.Invoke();
        PAF_Event[] _events = null; 
        while (m_currentGameTime <= m_gameDuration)
        {
            OnNextSecond?.Invoke(m_gameDuration - m_currentGameTime);
            _events = null;
            _events = m_gameEvents.ToList().Where(e => e.CallingTime <= m_currentGameTime && !e.HasBeenCalled).ToArray(); 
            if(_events != null && _events.Length > 0)
            {
                _events.ToList().ForEach(e => e.CallEvent()); 
            }
            yield return new WaitForSeconds(1.0f);
            m_currentGameTime++;
        }
        OnGameEnd?.Invoke(m_playerOneScore, m_playerTwoScore);
        m_playerOneIsReady = false;
        m_playerTwoIsReady = false;
        m_gameIsOver = true; 
    }

    /// <summary>
    /// Increase the score of the player according to the given points 
    /// </summary>
    /// <param name="_isFirstPlayer">Is the rewarded player is player one?</param>
    /// <param name="_fruitScore">The points to add to the player score</param>
    private void IncreasePlayerScore(bool _isFirstPlayer, int _fruitScore)
    {
        if (_isFirstPlayer)
        {
            m_playerOneScore += _fruitScore;
            OnPlayerScored?.Invoke(_isFirstPlayer, m_playerOneScore);
            return; 
        }
        m_playerTwoScore += _fruitScore;
        OnPlayerScored?.Invoke(_isFirstPlayer, m_playerTwoScore); 
    }

    /// <summary>
    /// Set if the player is ready
    /// If both players are ready, start the game
    /// </summary>
    /// <param name="_isPlayerOne"></param>
    public void SetPlayerReady(bool _isPlayerOne)
    {
        if (m_gameIsOver) return; 
        if (_isPlayerOne)
        {
            m_playerOneIsReady = true;
        }
        else
        {
            m_playerTwoIsReady = true;
        }

        OnPlayerReady?.Invoke(_isPlayerOne); 

        if(PlayersAreReadyToStart)
        { 
            StartCoroutine(StartVideoPlayer());
        }
    }

    /// <summary>
    /// Start the IncreasePlayingTime Coroutine
    /// </summary>
    public void StartGame()
    {
        m_gameIsReadyToStart = true;
        StartCoroutine(IncreasePlayingTime());
    }

    /// <summary>
    /// Disable the cinematic if it exists
    /// </summary>
    void HideCinematic()
    {
        if (!m_videoPlayer) return;
        m_videoPlayer.Stop();
        m_videoPlayer.enabled = false;
        m_videoPlayer.enabled = true;
        StartCoroutine(FadeCinematic());
    }

    IEnumerator FadeCinematic()
    {
        if (!m_introMaterial) yield break;
        while (m_introMaterial.color.a > 0)
        {
            m_introMaterial.color = Color.Lerp(m_introMaterial.color, new Color(m_introMaterial.color.r, m_introMaterial.color.g, m_introMaterial.color.b, 0), Time.deltaTime * m_speedFadeCinematic);
            yield return new WaitForSeconds(.1f);
        }
        yield return null;
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return; 
        }
    }

    private void Start()
    {
        PAF_Fruit.OnFruitEaten += IncreasePlayerScore;
        OnEndCinematic += HideCinematic;
    }

    private void OnDestroy()
    {
        PAF_Fruit.OnFruitEaten -= IncreasePlayerScore;
        OnEndCinematic -= HideCinematic;
    }
    #endregion

    #endregion
}
