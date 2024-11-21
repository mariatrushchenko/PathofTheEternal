using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    public static PlayerKnockback instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void KnockBackPlayer(Collider2D player, GameObject enemy)
    {
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // access player components to enable knockback
        PlayerMovement _player = player.GetComponent<PlayerMovement>();

        // give info about enemy location at point of collision to variable in PlayerMovement script
        _player.enemyFromKnockback = enemy.transform.position;

        // access player components to enable knockback
        _player.knockBackTimer = _player.knockBackTimerCountdown;

        // player will take no damage while knocked back
        _player.GetComponent<Player>().isInvulnerable = true;
    }
}
