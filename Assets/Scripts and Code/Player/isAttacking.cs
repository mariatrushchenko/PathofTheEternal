using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isAttacking : StateMachineBehaviour
{
    PlayerCombat pc;
    SpriteRenderer sr;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pc = animator.GetComponent<PlayerCombat>();
        sr = animator.GetComponent<SpriteRenderer>();

        pc.isAttacking = true;

        // check if player is looking left, attacking left sprites aren't included;
        pc.RotateLeft();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pc.isAttacking = false;
        sr.flipX = false;
    }
}
