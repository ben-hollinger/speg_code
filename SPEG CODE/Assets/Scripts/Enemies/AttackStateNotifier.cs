using UnityEngine;

public class AttackStateNotifier : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<EnemyController>();
        if (enemy != null) enemy.OnAttackStateEntered();

        var player = animator.GetComponentInParent<PlayerController>();
        if (player != null) player.OnAttackStateEntered();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<EnemyController>();
        if (enemy != null) enemy.OnAttackStateExited();

        var player = animator.GetComponentInParent<PlayerController>();
        if (player != null) player.OnAttackStateExited();
    }
}
