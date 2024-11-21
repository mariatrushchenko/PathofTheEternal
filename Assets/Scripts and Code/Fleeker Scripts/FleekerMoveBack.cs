using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleekerMoveBack : StateMachineBehaviour
{
    Fleeker f;
    Rigidbody2D rb;

    [SerializeField] float fleekerMoveSpeedMultiplier;

    [Header("Timer")]
    [SerializeField] float moveBackTimerC;
    float moveTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        f = animator.GetComponent<Fleeker>();
        rb = animator.GetComponentInParent<Rigidbody2D>();

        // initialize timer
        moveTimer = moveBackTimerC;

        // get player vector position to move back
        Vector2 playerVector = f.moveSpeed * fleekerMoveSpeedMultiplier * (animator.transform.position - f.player.position).normalized;
        rb.velocity = new Vector2(playerVector.x, playerVector.y);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (moveTimer <= 0)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("MoveBack", false);
        }
        else
            moveTimer -= Time.deltaTime;
    }
}
