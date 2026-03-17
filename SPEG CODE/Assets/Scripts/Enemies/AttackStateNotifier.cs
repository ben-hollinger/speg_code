using UnityEngine;

public class AttackStateNotifier : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<EnemyController>();
        if (enemy == null)
        {
            return;
        }

        enemy.OnAttackStateEntered();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.GetComponentInParent<EnemyController>();
        if (enemy == null)
        {
            return;
        }

        enemy.OnAttackStateExited();
    }
}

