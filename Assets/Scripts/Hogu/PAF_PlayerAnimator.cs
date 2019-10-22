using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator playerAnimator = null;



    public void Init(float _speed) => playerAnimator.SetFloat("start speed", _speed);

    public void SetMoving(bool _state)
    {
        if (!playerAnimator) return;
        playerAnimator.SetBool("move", !_state);
    }

    public void SetStunned()
    {
        if (!playerAnimator) return;
        playerAnimator.SetTrigger("stun");
    }

    public void SetAttack()
    {
        if (!playerAnimator) return;
        playerAnimator.SetTrigger("attack");
    }

    public void SetFalling()
    {
        if (!playerAnimator) return;
        playerAnimator.SetTrigger("fall");
    }



}
