using System;
using System.Collections;
using System.Collections.Generic;
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

    #endregion

    #region Fields / Properties
    public static PAF_GameManager Instance = null;

    [SerializeField, Range(1, 500)] private int gameDuration = 120;
    private int currentGameTime = 0;

    private int playerOneScore = 0;
    private int playerTwoScore = 0; 


	#endregion

	#region Methods

	#region Original Methods

	#endregion

	#region Unity Methods
    
	#endregion

	#endregion
}
