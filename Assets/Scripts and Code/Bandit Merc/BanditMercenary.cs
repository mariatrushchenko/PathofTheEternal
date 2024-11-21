using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditMercenary : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyKnockback kb;
    Animator animator;

    public float moveSpeed;
    [SerializeField] Transform interactorPoint;
    [SerializeField] float interactorRotationThreshold;

    [Header("Attack")]
    public bool isAttacking;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float CheckAttackRadius;

    [SerializeField] Transform attackPoint;
    [SerializeField] Transform attackVerticalPoint;
    [SerializeField] Vector2 attackRadius;
    [SerializeField] Vector2 attackRadiusVertical;

    [SerializeField] LayerMask playerMask;
    [SerializeField] float attackRate;
    float nextAttack;

    [Header("Damage")]
    [SerializeField] int damage;
    [SerializeField] int collisionDamage;

    [Header("Player In Range")]
    [SerializeField] float checkRadius;
    bool isInRange;
    bool inRangeAttack;

    // used for attacking
    Transform chosenAttackPoint;
    Vector2 horizOrVertRadius;

    // strings for minor optimization
    readonly string Movement = "Movement";
    readonly string Horizontal = "Horizontal";
    readonly string Vertical = "Vertical";
    readonly string LastHorizontal = "LastHorizontal";
    readonly string LastVertical = "LastVertical";

    public Transform player;
    float nextS;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        kb = GetComponent<EnemyKnockback>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {   
        if (player != null)
        {
            if (kb.knockBackTimer <= 0)
            {
                if (isInRange == true)
                {
                    Vector2 moveDir = (player.position - transform.position).normalized * moveSpeed;
                    rb.velocity = new Vector2(moveDir.x, moveDir.y);

                    // animator blend tree
                    animator.SetFloat(Horizontal, rb.velocity.x);
                    animator.SetFloat(Vertical, rb.velocity.y);
                }
                else
                    rb.velocity = Vector2.zero;
            }
            else
                kb.Knockback(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {   
            FindPlayer();
            return;
        }

        // check if player is in range for: 1.) Moving towards and 2.) Attacking
        float playerDistance = (player.position - transform.position).sqrMagnitude;
        isInRange = playerDistance <= checkRadius * checkRadius;
        inRangeAttack = playerDistance <= CheckAttackRadius * CheckAttackRadius;

        // trigger attack
        if (inRangeAttack == true && nextAttack <= Time.time)
        {
            animator.SetTrigger("Attack");
            audioSource.Play();
            nextAttack = Time.time + attackRate;
        }

        if (isAttacking == false)
        {
            AnimatorBlendTree();
            InteractorRotation();
        }
    }

    void AnimatorBlendTree()
    {
        // animator blend tree
        // NOTE: We use rb.velocity.sqrMagnitude because if the enemy moves, velocity will always be > 0 (this makes it 
        // good for detecting movement via code)
        animator.SetFloat(Movement, rb.velocity.sqrMagnitude);

        // store last movement value in variables to determine which IDLE animation will play
        if (rb.velocity.sqrMagnitude != 0)
        {
            animator.SetFloat(LastHorizontal, rb.velocity.x);
            animator.SetFloat(LastVertical, rb.velocity.y);
        }
    }

    // rotate interactor based on Rigidbody2D Velocity
    void InteractorRotation()
    {   
        if (rb.velocity.x > interactorRotationThreshold)
            interactorPoint.localRotation = Quaternion.Euler(0, 0, 90);
        else if (rb.velocity.x < -interactorRotationThreshold)
            interactorPoint.localRotation = Quaternion.Euler(0, 0, -90);
        else if (rb.velocity.y > interactorRotationThreshold)
            interactorPoint.localRotation = Quaternion.Euler(0, 0, 180);
        else if (rb.velocity.y < interactorRotationThreshold)
            interactorPoint.localRotation = Quaternion.Euler(0, 0, 0);
    }

    // ----------------------------
    // Attacking
    // called on animation frame
    void Attack()
    {   
        // assign correct attackPoints and radius based on which way the enemy is facing (1st is sideways and else is top/down)
        if (interactorPoint.localRotation == Quaternion.Euler(0, 0, -90) || interactorPoint.localRotation == Quaternion.Euler(0, 0, 90))
        {
            horizOrVertRadius = attackRadiusVertical;
            chosenAttackPoint = attackVerticalPoint;
        }
        else
        {
            horizOrVertRadius = attackRadius;
            chosenAttackPoint = attackPoint;
        }

        Collider2D player = Physics2D.OverlapBox(chosenAttackPoint.position, horizOrVertRadius, 0f, playerMask);
        if (player != null)
        {
            player.GetComponent<Player>().DamagePlayer(damage);
            PlayerKnockback.instance.KnockBackPlayer(player, gameObject);
        }
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
        if (nextS <= Time.time)
        {
            if (rb.velocity.sqrMagnitude > 0)
                rb.velocity = Vector2.zero;

            animator.SetFloat(LastHorizontal, 0);
            animator.SetFloat(LastVertical, 0);

            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextS = Time.time + 1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        Gizmos.DrawWireSphere(transform.position, CheckAttackRadius);
        //Gizmos.DrawWireCube(attackPoint.position, attackRadius);
        Gizmos.DrawWireCube(attackVerticalPoint.position, attackRadiusVertical);
    }
}
