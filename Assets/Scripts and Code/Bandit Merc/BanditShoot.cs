using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditShoot : StateMachineBehaviour
{
    MercenaryBoss mb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mb = animator.GetComponent<MercenaryBoss>();
        mb.isAttacking = true;

        AudioManager.instance.Play("Mercenary Shoot");

        if (mb.player.position.x > animator.transform.position.x)
        {
            animator.SetFloat("Horizontal", 1);
            mb.interactorPoint.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            animator.SetFloat("Horizontal", -1);
            mb.interactorPoint.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Shoot", false);
        mb.isAttacking = false;
    }
}
