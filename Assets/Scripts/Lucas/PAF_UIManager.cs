using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// UI animator, making all different menus and sub-menus transitions.
    /// </summary>
    [SerializeField] private Animator animator = null;
    #endregion

    #region Methods

    #region Original Methods
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
        animator.SetInteger("State", _state);
    }
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #endregion
}
