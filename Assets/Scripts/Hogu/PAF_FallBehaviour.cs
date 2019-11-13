using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_FallBehaviour : StateMachineBehaviour
{
    private Transform m_parentTransform;  
    private float m_time = 0;
    private Vector3 m_targetedPosition;
    private Vector3 m_basePosition;

    [SerializeField] private AnimationCurve m_fallingCurve = null;
    [SerializeField] private AnimationCurve m_zCurve = null;
    [SerializeField] private AnimationCurve m_scaleCurve = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_parentTransform) m_parentTransform = animator.transform.parent;
        m_time = 0;
        if (m_parentTransform)
        {
            m_targetedPosition = m_parentTransform.position + new Vector3(0, -10f, 5f);
            m_basePosition = m_parentTransform.position; 
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_parentTransform) return;
        m_time += Time.deltaTime;
        m_parentTransform.transform.position = new Vector3(m_basePosition.x, Mathf.Lerp(m_basePosition.y, m_targetedPosition.y, m_fallingCurve.Evaluate(m_time)), Mathf.Lerp(m_basePosition.z, m_targetedPosition.z, m_zCurve.Evaluate(m_time)));
        animator.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * .5f, m_scaleCurve.Evaluate(m_time)); 
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<PAF_Player>().Invoke("Respawn", .5f);
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
