using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryBullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] GameObject particleEffect;

    [Header("Speed Direction is determined in Mercenary Boss Script")]
    public float moveSpeed;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(moveSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            Effect(collision);
        }
        else if (collision.CompareTag("BG Collider"))
            Effect(collision);
    }

    void Effect(Collider2D collision)
    {
        Vector2 spawnPos = collision.ClosestPoint(transform.position);
        GameObject effect = Instantiate(particleEffect, spawnPos, Quaternion.identity);
        Destroy(effect, 0.5f);

        Destroy(gameObject);
    }    
}
