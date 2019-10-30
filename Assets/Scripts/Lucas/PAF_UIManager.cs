using UnityEngine;
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
    /// <summary>
    /// Screen UI animator, making all different menus and sub-menus transitions.
    /// </summary>
    [SerializeField] private Animator screenAnimator = null;

    /// <summary>
    /// World UI animator, playing animations for score & timer in world space.
    /// </summary>
    [SerializeField] private Animator worldAnimator = null;


    /// <summary>
    /// Text used to display first player's score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI playerOneScore = null;

    /// <summary>
    /// Text used to display second player's score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI playerTwoScore = null;
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
        }
        else
        {
            playerTwoScore.text = _score.ToString();
            //worldAnimator.SetTrigger("Score P2");
            screenAnimator.SetTrigger("P2 Score");
        }
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
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        PAF_GameManager.OnPlayerScored += SetPlayerScore; 

    }
    #endregion

    #endregion
}
