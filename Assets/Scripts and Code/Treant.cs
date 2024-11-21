using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treant : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    EnemyKnockback kb;
    SpriteRenderer sr;

    [SerializeField] float moveSpeed;
    [SerializeField] int collisionDamage;
    [SerializeField] Vector3[] waypoints;
    int index;

    [Header("Player detection")]
    [SerializeField] bool playerInRange;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask playerMask;

    Vector3 moveVector;
    [SerializeField] bool movingToCurrentWaypoint;

    Transform player;
    float nextSearch;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        kb = GetComponent<EnemyKnockback>();
        sr = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        Physics2D.IgnoreLayerCollision(8, 8);

        index = Random.Range(0, waypoints.Length);
        transform.position = waypoints[index];
        ChooseNextIndex();  // comment this code out when you only have one position and want to test something in inspector
    }

    private void FixedUpdate()
    {   
        if (kb.knockBackTimer <= 0)
        {   
            // if player is in range, start moving to them. IF NOT, move to waypoint
            if (playerInRange == true)
            {
                MoveTowardPosition(player.position);
                movingToCurrentWaypoint = false;
            }
            else
            {   
                // if statement is so that MoveTowardPosition function isnt called per F.U frame.
                if (movingToCurrentWaypoint == false)
                {
                    MoveTowardPosition(waypoints[index]);
                    movingToCurrentWaypoint = true;
                }

                // when treant reaches within a small range of a waypoint, find a NEW waypoint
                // use (A-B).sqrMagnitude and squaring the other side to avoid using the taxing sqr root operation
                if ((transform.position - waypoints[index]).sqrMagnitude <= 0.25f * 0.25f) 
                {
                    ChooseNextIndex();

                    // set bool to false so that the new waypoint vector can be calculated for a frame
                    movingToCurrentWaypoint = false;
                }
            }

            // move treant
            rb.velocity = new Vector2(moveVector.x, moveVector.y);

            if (rb.velocity.x < 0)
                sr.flipX = true;
            else
                sr.flipX = false;
        }
        else
            kb.Knockback(gameObject);
    }

    private void Update()
    {
        // when player dies, move to waypoint 
        if (player == null)
        {
            FindPlayer();
            rb.velocity = Vector2.zero;
            return;
        }


        if (kb.knockBackTimer <= 0)
            playerInRange = Physics2D.OverlapCircle(transform.position, checkRadius, playerMask);
    }

    void ChooseNextIndex()
    {
        int nextIndex = Random.Range(0, waypoints.Length);
        if (nextIndex != index)
        {
            // set new index
            index = nextIndex;
        }
        else
        {
            // recursive to avoid going to same index
            ChooseNextIndex();
        }
    }

    void MoveTowardPosition(Vector3 objectPosition)
    {   
        // find direction
        Vector3 vector = (objectPosition - transform.position).normalized;

        // get angle so correct animation can be played 
        float direction = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;  // mathf.atan2 returns in radians
        animator.SetFloat("Movement", direction);

        // define moveVector to be used in FixedUpdate
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
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
