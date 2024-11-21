using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeSpawn : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject healthBarCanvas = animator.transform.parent.GetChild(0).gameObject;
        healthBarCanvas.SetActive(true);

        animator.GetComponent<Collider2D>().enabled = true;
        animator.GetComponent<SlimeMinion>().enabled = true;
    }
}
