using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chicken_run : StateMachineBehaviour
{
    ChickenPatrol cp;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cp = animator.GetComponent<ChickenPatrol>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator a, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        // move to next waypoint
        a.transform.position = Vector2.MoveTowards(a.transform.position, cp.waypoints[cp.index], cp.moveSpeed * Time.deltaTime);

        // make chicken wait possible idle time (code now goes to chicken_idle.cs)
        if (cp.transform.position == cp.waypoints[cp.index])
            a.SetBool("Idle", true);
    }
}
