using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossDash : StateMachineBehaviour
{
    SlimeBoss sb;
    Rigidbody2D rb;
    EnemyKnockback kb;

    [SerializeField] float dashTimerC;
    [SerializeField] float dashTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sb = animator.GetComponent<SlimeBoss>();
        rb = animator.GetComponentInParent<Rigidbody2D>();
        kb = animator.GetComponent<EnemyKnockback>();

        // initialize timer
        dashTimer = dashTimerC;

        // find player position vector direction and move towards it
        Vector2 playerDirection = (sb.player.position - animator.transform.parent.position).normalized * sb.dashSpeed;
        rb.velocity = new Vector2(playerDirection.x, playerDirection.y);

        Physics2D.IgnoreLayerCollision(8, 8, true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // avoid getting knocked back during dash
        if (kb.knockBackTimer > 0)
            kb.knockBackTimer = 0;

        if (dashTimer <= 0)      
            animator.SetBool("Dash", false);      
        else
            dashTimer -= Time.deltaTime;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // reset velocity
        rb.velocity = Vector2.zero;

        Physics2D.IgnoreLayerCollision(8, 8, false);
    }
}
