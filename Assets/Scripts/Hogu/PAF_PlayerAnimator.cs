using UnityEngine;

public class PAF_PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator playerAnimator = null;

    private readonly int startSpeed_Hash = Animator.StringToHash("start speed");
    private readonly int move_Hash = Animator.StringToHash("move");
    private readonly int stun_Hash = Animator.StringToHash("stun");
    private readonly int attack_Hash = Animator.StringToHash("attack");
    private readonly int fall_Hash = Animator.StringToHash("fall");

    private void Start()
    {
        AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);
        playerAnimator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }

    public void Init(float _speed) => playerAnimator.SetFloat(startSpeed_Hash, _speed);

    public void SetMoving(bool _state) => playerAnimator.SetBool(move_Hash, !_state);

    public void SetStunned() => playerAnimator.SetTrigger(stun_Hash);

    public void SetAttack() => playerAnimator.SetTrigger(attack_Hash);

    public void SetFalling() => playerAnimator.SetTrigger(fall_Hash);
}
