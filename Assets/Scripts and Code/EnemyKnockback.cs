using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{   
    // Attach this script to enemy that you want to have a knockback

    [Header("Dont Change These! READONLY")]
    public float knockBackTimer;
    public Vector3 knockBackSourcePosition;

    [Header("Adjustable Values")]
    public float knockBackTimerCountdown = 0.1f;
    public float knockBackForce = 12.5f;

    /// <summary>
    /// Knockbacks the enemy based on player position's Vector3 at start time of code.
    /// </summary>
    /// <param name="enemyGameObject"></param>
    public void Knockback(GameObject enemyGameObject)
    {
        knockBackTimer -= Time.fixedDeltaTime;

        Vector2 dir = (enemyGameObject.transform.position - knockBackSourcePosition).normalized * knockBackForce;
        enemyGameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(dir.x, dir.y);
    }
}
