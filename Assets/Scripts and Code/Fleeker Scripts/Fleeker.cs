using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleeker : MonoBehaviour
{
    EnemyKnockback kb;
    Rigidbody2D rb;
    AudioSource shootSound;

    public float moveSpeed;

    [Header("Attack: Attack Spd is Time.time + attackRate")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform attackPoint;
    public float attackRate;
    [HideInInspector] public float nextTimeAttack;

    [Header("Player in Range")]
    public float checkRadius;
    public LayerMask playerMask;
    public float minimumRange;

    [HideInInspector] public Transform player;
    float nextS;

    [Header("If the object is facing right, toggle isFlipped to true")]
    public bool isFlipped;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        kb = GetComponent<EnemyKnockback>();
        rb = GetComponentInParent<Rigidbody2D>();
        shootSound = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (kb.knockBackTimer > 0)
            kb.Knockback(transform.parent.gameObject);
        else if (rb.velocity.sqrMagnitude > 0)
            rb.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }
    }

    /// <summary>
    /// This alternative to new Vector3(-1, 1, 1) is better because there is no stuttering. Uses transform.Rotation
    /// instead of transform.localScale.
    /// </summary>
    public void FacePlayer()
    {
        // when the player is to the LEFT and isFlipped is true, ROTATE 180 DEGREES
        if (player.position.x < transform.position.x && isFlipped == true)
        {
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        // when the player is to the RIGHT and isFlipped is false, ROTATE 180 DEGREES
        else if (player.position.x > transform.position.x && isFlipped == false)
        {
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    // called in animation frame
    void SpawnProjectile()
    {
        Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        shootSound.Play();
    }

    // ----------------------------------------------------
    void FindPlayer()
    {
        if (nextS <= Time.time)
        {
            GameObject r = GameObject.FindGameObjectWithTag("Player");
            if (r != null)
                player = r.transform;
            nextS = Time.time + 1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.parent.position, checkRadius);
        Gizmos.DrawWireSphere(transform.parent.position, minimumRange);
    }
}
