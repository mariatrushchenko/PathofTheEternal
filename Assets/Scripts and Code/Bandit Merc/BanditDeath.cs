using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditDeath : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.instance.Play("Enemy Death");

        // destroy components
        Destroy(animator.GetComponent<Collider2D>());
        Destroy(animator.GetComponent<Rigidbody2D>());
        Destroy(animator.GetComponent<MercenaryBoss>());
        Destroy(animator.GetComponent<EnemyKnockback>());

        // destroy health bar
        GameObject canvasObject = animator.transform.GetChild(0).gameObject;
        Destroy(canvasObject);

        // remove object from enemies list so walls can go back after boss fight
        BossFight bf = FindObjectOfType<BossFight>();
        bf.enemies.Remove(animator.gameObject);
    }
}
