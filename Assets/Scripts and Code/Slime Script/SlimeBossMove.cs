using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossMove : StateMachineBehaviour
{
    SlimeBoss sb;
    EnemyKnockback kb;

    Vector3 position;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sb = animator.GetComponent<SlimeBoss>();
        kb = animator.GetComponent<EnemyKnockback>();

        sb.dashTimer = sb.dashTimerC;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator a, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (sb.player == null)
            return;

        // timer to trigger dash code
        if (sb.dashTimer <= 0)
        {   
            // move to offset position.
            a.transform.parent.position = Vector2.MoveTowards(a.transform.parent.position, position, sb.moveSpeed * 1.5f * Time.deltaTime);

            // once slime boss is within range of offset position, transition over to SlimeBossDash.cs for dashing code.
            if ((position - a.transform.parent.position).sqrMagnitude <= 0.5f * 0.5f)
                a.SetBool("Dash", true);
        }
        else
        {
            sb.dashTimer -= Time.deltaTime;
            if (sb.dashTimer <= 0)
            {
                // find random offset position. (EDGE CASE: position is in wall. Use Recursion)
                FindValidPosition();
            }

            if (kb.knockBackTimer <= 0)
            {
                // move towards player
                a.transform.parent.position = Vector2.MoveTowards(a.transform.parent.position,
                    sb.player.position, sb.moveSpeed * Time.deltaTime);

                sb.FacePlayer();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        // reset timer once transitioning out
        sb.dashTimer = sb.dashTimerC;
    }

    void FindValidPosition()
    {   
        // find a random offest position and check if its able to be moved to
        Vector2 possiblePosition = sb.GoToOffsetPosition();
        bool isValid = ValidPosition(possiblePosition);

        if (isValid == true)
            position = possiblePosition;
        else
            FindValidPosition();
    }

    bool ValidPosition(Vector2 position)
    {
        for (int i = 0; i < sb.invalidColliders.Length; i++)
        {
            Vector3 centerPoint = sb.invalidColliders[i].bounds.center;
            float width = sb.invalidColliders[i].bounds.extents.x;
            float height = sb.invalidColliders[i].bounds.extents.y;

            float leftExtent = centerPoint.x - width;
            float rightExtent = centerPoint.x + width;
            float lowerExtent = centerPoint.y - height;
            float upperExtent = centerPoint.y + height;

            if (position.x >= leftExtent && position.x <= rightExtent)
            {
                if (position.y >= lowerExtent && position.y <= upperExtent)
                    return false;
            }
        }

        return true;
    }
}
