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
    #endregion

    #region Fields / Properties
    public static PAF_GameManager Instance = null;

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
	#endregion

	#region Methods

	#region Original Methods
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
        if(_isFirstPlayer)
        {
            m_playerOneScore += _fruitScore;
            PAF_UIManager.Instance?.SetPlayerScore(m_playerOneScore, true);
            return; 
        }
        m_playerTwoScore += _fruitScore;
        PAF_UIManager.Instance?.SetPlayerScore(m_playerOneScore, false);
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        StartCoroutine(IncreasePlayingTime());
        PAF_Fruit.OnFruitEaten += IncreasePlayerScore; 
    }
    #endregion

    #endregion
}
