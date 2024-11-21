using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    EnemyKnockback kb;
    SpriteRenderer sr;

    [SerializeField] bool playerInRange;
    [SerializeField] int collisionDamage;
    [SerializeField] float moveSpeed;
    [SerializeField] float playerCheckRadius;

    Transform player;
    float nextSearch;

    Vector2 moveVector;

    readonly string Run = "Run";
    readonly string Movement = "Movement";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        kb = GetComponent<EnemyKnockback>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (kb.knockBackTimer <= 0)
        {
            if (playerInRange == true)
                rb.velocity = new Vector2(moveVector.x, moveVector.y);

            if (moveVector.x < 0)
                sr.flipX = true;
            else
                sr.flipX = false;
        }
        else
            kb.Knockback(gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (kb.knockBackTimer <= 0)
        {
            playerInRange = (transform.position - player.position).sqrMagnitude <= playerCheckRadius * playerCheckRadius;
            if (playerInRange == true)
            {
                animator.SetBool(Run, true);
                MoveTowardPlayer();
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool(Run, false);
            }
        }
    }
    
    void MoveTowardPlayer()
    {
        Vector2 vector = (player.position - transform.position).normalized;
        float direction = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;  // mathf.atan2 returns in radians
        animator.SetFloat(Movement, direction);

        moveVector = vector * moveSpeed;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.DamagePlayer(collisionDamage);
            PlayerKnockback.instance.KnockBackPlayer(collision.collider, gameObject);
        }
    }

    void FindPlayer()
    {
        if (nextSearch <= Time.time)
        {
            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextSearch = Time.time + 1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, playerCheckRadius);
    }
}
