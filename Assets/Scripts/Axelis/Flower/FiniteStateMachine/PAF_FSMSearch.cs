﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_FSMSearch : StateMachineBehaviour
{
    private PAF_Flower m_owner = null;
    private Coroutine m_behaviourCoroutine = null; 


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_owner) m_owner = animator.GetComponent<PAF_Flower>();
        if (!m_owner) return;
        m_behaviourCoroutine = m_owner.StartCoroutine(m_owner.GetClosestFruit()); 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(m_behaviourCoroutine != null)
        {
            m_owner.StopCoroutine(m_owner.GetClosestFruit());
            m_behaviourCoroutine = null;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}