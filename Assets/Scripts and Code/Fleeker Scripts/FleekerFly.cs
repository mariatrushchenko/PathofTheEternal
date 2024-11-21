using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleekerFly : StateMachineBehaviour
{
    Fleeker f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        f = animator.GetComponent<Fleeker>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (f.player == null)
            return;

        bool playerInRange = Physics2D.OverlapCircle(animator.transform.parent.position, f.checkRadius, f.playerMask);
        if (playerInRange == true)
        {
            f.FacePlayer();

            // check if player is inside the minimum range (inner circle)
            bool isInsideMinRange = Physics2D.OverlapCircle(animator.transform.parent.position, f.minimumRange, f.playerMask);
            if (isInsideMinRange == false)
            {
                // move toward player
                animator.transform.parent.position = Vector2.MoveTowards(animator.transform.parent.position, f.player.position,
                    f.moveSpeed * Time.deltaTime);
            }
            else if (f.nextTimeAttack <= Time.time)
            {      
                f.nextTimeAttack = Time.time + f.attackRate;
                animator.SetTrigger("Attack");               
            }
        }
    }
}
