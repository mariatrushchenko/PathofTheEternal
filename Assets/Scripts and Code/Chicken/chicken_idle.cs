using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chicken_idle : StateMachineBehaviour
{
    ChickenPatrol cp;
    float idleTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cp = animator.GetComponent<ChickenPatrol>();
        idleTimer = Random.Range(0, cp.maxIdleTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        // have chicken wait at current waypoint for idleTimer seconds, then find new waypoint to go to
        if (idleTimer <= 0)
        {
            cp.ChooseNextIndex();
            animator.SetBool("Idle", false);
        }
        else
            idleTimer -= Time.deltaTime;
    }
}
