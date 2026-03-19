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
        // If we are transitioning from one attack-tagged state to another, do not treat
        // it as an attack end (this prevents prematurely ending combos).
        var next = animator.GetNextAnimatorStateInfo(layerIndex);
        if (next.IsTag("attack"))
            return;

        var enemy = animator.GetComponentInParent<EnemyController>();
        if (enemy != null) enemy.OnAttackStateExited();

        var player = animator.GetComponentInParent<PlayerController>();
        if (player != null) player.OnAttackStateExited();
    }
}
