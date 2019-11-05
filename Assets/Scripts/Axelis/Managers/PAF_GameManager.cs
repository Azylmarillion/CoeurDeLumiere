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
    /// </summary>
    public static event Action OnGameEnd = null;
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

    [SerializeField] private UnityEngine.Video.VideoPlayer m_videoPlayer = null; 
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Start the video player to start the cinematic
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartVideoPlayer()
    {
        m_videoPlayer.Play();
        while (m_videoPlayer.isPlaying)
        {
            yield return new WaitForSeconds(.1f); 
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
        while (m_currentGameTime < m_gameDuration)
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
        OnGameEnd?.Invoke(); 
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
            if(m_videoPlayer)
            {
                StartCoroutine(StartVideoPlayer());
                return; 
            }
            OnEndCinematic?.Invoke();
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
    }

    private void OnDestroy()
    {
        PAF_Fruit.OnFruitEaten -= IncreasePlayerScore; 
    }
    #endregion

    #endregion
}
