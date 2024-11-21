using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMercenary : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.instance.Play("Enemy Death");

        // destroy components
        Destroy(animator.GetComponent<Collider2D>());
        Destroy(animator.GetComponent<Rigidbody2D>());
        Destroy(animator.GetComponent<BanditMercenary>());
        Destroy(animator.GetComponent<EnemyKnockback>());

        // destroy health bar
        GameObject canvasObject = animator.transform.GetChild(0).gameObject;
        Destroy(canvasObject);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
