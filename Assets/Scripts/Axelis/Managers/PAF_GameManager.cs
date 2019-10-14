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
    public static event Action OnGameStart = null;
    public static event Action OnGameEnd = null; 
    #endregion

    #region Fields / Properties
    public static PAF_GameManager Instance = null;

    [SerializeField, Range(1, 500)] private int m_gameDuration = 120;
    private int m_currentGameTime = 0;

    private int m_playerOneScore = 0;
    private int m_playerTwoScore = 0;

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
            _events = null;
            yield return new WaitForSeconds(1.0f) ;
            _events = m_gameEvents.ToList().Where(e => e.CallingTime <= m_currentGameTime && !e.HasBeenCalled).ToArray(); 
            if(_events != null && _events.Length > 0)
            {
                _events.ToList().ForEach(e => e.CallEvent()); 
            }
            m_currentGameTime ++; 
        }
        OnGameEnd?.Invoke(); 
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        StartCoroutine(IncreasePlayingTime()); 
    }
    #endregion

    #endregion
}
