using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryBoss : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyKnockback kb;
    Enemy enemy;
    Animator animator;

    public float moveSpeed;
    public Transform interactorPoint;
    [SerializeField] float interactorRotationThreshold;

    [Header("Melee Attack")]
    [SerializeField] AudioSource meleeAudioSource;
    [SerializeField] float CheckAttackRadius;

    [SerializeField] Transform attackPoint;
    [SerializeField] Transform attackVerticalPoint;
    [SerializeField] Vector2 attackRadius;
    [SerializeField] Vector2 attackRadiusVertical;

    [SerializeField] LayerMask playerMask;
    [SerializeField] float attackRate;
    float nextAttack;
    
    [HideInInspector] public bool isAttacking;

    [Header("Damage")]
    [SerializeField] int damage;
    [SerializeField] int collisionDamage;

    [Header("Roll and Shoot Timer")]
    [SerializeField] float dashSpeed;
    [SerializeField] float playerCheckAxisOffset;
    [SerializeField] float minimumRollDistance;
    [SerializeField] float rollTimerC;
    float rollTimer;

    [Header("Bullet for shooting")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject homingBulletPrefab;

    Vector3 playerPositionY;
    bool inRangeAttack;
    bool findPlayerPos = true;

    // used for attacking. Determines if enemy is using standard attack points and radius or vertical ones.
    Transform chosenAttackPoint;
    Vector2 horizOrVertRadius;

    // strings for minor optimization
    readonly string Movement = "Movement";
    readonly string Horizontal = "Horizontal";
    readonly string Vertical = "Vertical";
    readonly string LastHorizontal = "LastHorizontal";
    readonly string LastVertical = "LastVertical";
    readonly string Roll = "Roll";
    readonly string Shoot = "Shoot";
    readonly string ShootLeft = "ShootLeft";
    readonly string ShootRight = "ShootRight";

    [HideInInspector] public Transform player;
    float nextS;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        kb = GetComponent<EnemyKnockback>();
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();

        rollTimer = rollTimerC;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {   
            // if the enemy is not knocked back
            if (kb.knockBackTimer <= 0)
            {   
                // when roll interval timer hits 0 AND enemy is not attacking, start the roll
                if (rollTimer <= 0 && isAttacking == false)
                {
                    // If the player is within range on the X axis, just shoot and ignore rolling completely. If not,
                    // roll toward player Y position and shoot.
                    if (player.position.y <= transform.position.y + playerCheckAxisOffset && player.position.y >= transform.position.y - playerCheckAxisOffset)
                    {
                        ShootGun();
                    }
                    else
                    {
                        // find player position on that specific frame (only find player's position vector once in FixedUpdate)
                        if (findPlayerPos == true)
                        {
                            playerPositionY = new Vector2(transform.position.x, player.position.y);

                            // set animator and Rigidbody2D velocity once to roll to position
                            animator.SetBool(Roll, true);
                            MoveToPosition(playerPositionY, dashSpeed);

                            findPlayerPos = false;
                        }

                        // when enemy is within reasonable distance, STOP ROLLING and fire a bullet
                        if ((playerPositionY - transform.position).sqrMagnitude <= minimumRollDistance * minimumRollDistance)
                            ShootGun();
                    }
                }
                else
                {   
                    // run towards player while counting down timer
                    MoveToPosition(player.position, moveSpeed);
                    rollTimer -= Time.deltaTime;
                }

                // only constantly update the animator blend tree when not attacking or rolling
                if (isAttacking == false && animator.GetBool(Roll) == false && animator.GetBool(Shoot) == false)
                {
                    // animator blend tree
                    animator.SetFloat(Horizontal, rb.velocity.x);
                    animator.SetFloat(Vertical, rb.velocity.y);
                }
            }
            else
                kb.Knockback(gameObject);
        }
    }

    bool enragedMode;

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // when boss is below certain percentage hp, decrease interval for shooting
        if (enragedMode == false && enemy.enemyStats.currentHealth <= enemy.enemyStats.maxHealth * .35f)
        {
            rollTimerC -= 1;
            enragedMode = true;
        }

        // check if player is in range for attacking
        float playerDistance = (player.position - transform.position).sqrMagnitude;
        inRangeAttack = playerDistance <= CheckAttackRadius * CheckAttackRadius;

        // trigger attack when player is in range and is not rolling
        if (inRangeAttack == true && nextAttack <= Time.time && 
            animator.GetBool(Roll) == false && animator.GetBool(Shoot) == false)
        {
            animator.SetTrigger("Attack");
            meleeAudioSource.Play();
            nextAttack = Time.time + attackRate;
        }

        if (isAttacking == false)
        {
            AnimatorBlendTree();
            InteractorRotation();
        }
    }

    // ------------------------------------
    // code used for rolling in FixedUpdate
    void ShootGun()
    {   
        // find player vector position again when timer hits 0 in FixedUpdate
        findPlayerPos = true;

        // begin shoot animation
        animator.SetBool(Roll, false);
        animator.SetBool(Shoot, true);

        // trigger animation that correctly faces the player 
        if (player.position.x > transform.position.x)
            animator.SetTrigger(ShootRight);
        else
            animator.SetTrigger(ShootLeft);

        // reset timer
        rollTimer = rollTimerC;
    }

    void MoveToPosition(Vector3 position, float speed)
    {
        Vector2 moveDirection = (position - transform.position).normalized * speed;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
    }

    // ---------------------------------------
    // called in animation frame for shooting
    void ShootBullet()
    {
        // choose between homing or linear (33% for homing)
        GameObject chosenPrefab;
        int random = Random.Range(0, 3);
        if (random == 0)
            chosenPrefab = homingBulletPrefab;
        else
            chosenPrefab = bulletPrefab;

        GameObject bullet = Instantiate(chosenPrefab, interactorPoint.transform.GetChild(0).position, Quaternion.identity);
        if (interactorPoint.localRotation == Quaternion.Euler(0, 0, -90) && chosenPrefab == bulletPrefab)
            bullet.GetComponent<MercenaryBullet>().moveSpeed *= -1;
    }

    // ---------------------------------------

    void AnimatorBlendTree()
    {
        // animator blend tree
        // NOTE: We use rb.velocity.sqrMagnitude because if the enemy moves, velocity will always be > 0 (this makes it 
        // good for detecting movement via code)
        animator.SetFloat(Movement, rb.velocity.sqrMagnitude);

        // store last movement value in variables to determine which IDLE animation will play
        if (rb.velocity.sqrMagnitude != 0 && animator.GetBool(Shoot) == false)
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
        // assign correct attackPoints and radius based on which way the enemy is facing (if statement is sideways and else is top/down)
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

    // ----------------------------------------------------

    void FindPlayer()
    {
        if (nextS <= Time.time)
        {
            if (rb.velocity.sqrMagnitude > 0)
                rb.velocity = Vector2.zero;

            animator.SetFloat(LastHorizontal, 0);
            animator.SetFloat(LastVertical, 0);

            GameObject sResult = GameObject.FindGameObjectWithTag("Player");
            if (sResult != null)
                player = sResult.transform;
            nextS = Time.time + 1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, CheckAttackRadius);
        //Gizmos.DrawWireCube(attackPoint.position, attackRadius);
        Gizmos.DrawWireCube(attackVerticalPoint.position, attackRadiusVertical);
    }
}
