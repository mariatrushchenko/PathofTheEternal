using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    ManaBar manaBar;
    PlayerStats stats;
    Animator animator;
    PlayerMovement playerMove;
    SpriteRenderer sr;

    public bool isAttacking;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] Transform interactor;

    [Header("Bow Attack")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowSpeed;
    [SerializeField] float arrowAttackSpeed;
    float nextArrowAttack = 0f;

    [Header("Punch Attack")]
    [SerializeField] Transform attackPoint;
    [SerializeField] Vector2 attackRadius;
    [SerializeField] float punchAttackSpeed;
    [SerializeField] GameObject punchEffect;
    float nextPunchAttack = 0f;

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.instance;
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();

        manaBar = GameObject.FindGameObjectWithTag("PlayerMana").GetComponent<ManaBar>();

        // initialize MANA
        stats.currentMana = stats.maxMana;

        manaBar.SetMaxMana(stats.maxMana);
        manaBar.SetCurrentMana(stats.maxMana);
    }

    /// <summary>
    /// Function is the basis for detecting enemy and damaging them. It checks if the enemy has EnemyKnockback.cs attached to it or not.
    /// </summary>
    /// <param name="enemy"></param>
    void DamageEnemy(Collider2D enemy, int damage)
    {
        if (enemy.TryGetComponent<EnemyKnockback>(out _) == true)
        {
            EnemyKnockback kb = enemy.GetComponent<EnemyKnockback>();
            kb.knockBackTimer = kb.knockBackTimerCountdown;
            kb.knockBackSourcePosition = transform.position;
        }

        enemy.GetComponent<Enemy>().DamageEnemy(damage);
    }

    // Update is called once per frame
    void Update()
    {   
        if (PauseMenu.GameIsPaused == false)
        {
            // the playerMove.canMove bool is targeted for when player is shopping and moving is not allowed.
            // player cant attack while also dashing.
            if (isAttacking == false && playerMove.canMove == true && playerMove.isDashing == false)
            {
                ArrowAttack();
                PunchAttack();
            }
        }
    }

    // ------------------------------------
    // Arrow

    void ArrowAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse1))
        {
            if (nextArrowAttack <= Time.time && stats.currentMana >= stats.arrowManaCost)
            {
                animator.SetTrigger("Bow");
                AudioManager.instance.Play("Bow Release");
                nextArrowAttack = Time.time + arrowAttackSpeed;

                // update mana
                stats.currentMana -= stats.arrowManaCost;
                manaBar.SetCurrentMana(stats.currentMana);
            }
        }
    }

    // called in animation frame
    void InstantiateArrow()
    {
        Physics2D.IgnoreLayerCollision(6, 7);   // avoid colliding with arrow

        GameObject arrow = Instantiate(arrowPrefab, interactor.position, Quaternion.identity);

        // make arrow face and go in the right direction
        Rigidbody2D arrowRB = arrow.GetComponent<Rigidbody2D>();
        if (interactor.eulerAngles.z == 0)
        {
            arrowRB.velocity = new Vector2(0f, -arrowSpeed);
            arrow.transform.Rotate(0f, 0f, 180f);
        }
        else if (interactor.eulerAngles.z == 180)
        {
            // arrow points up in its default state so no need to rotate sprite
            arrowRB.velocity = new Vector2(0f, arrowSpeed);
        }
        else if (interactor.eulerAngles.z == 90)
        {
            arrowRB.velocity = new Vector2(arrowSpeed, 0f);
            arrow.transform.Rotate(0f, 0f, -90);
        }
        else if (interactor.eulerAngles.z == 270)
        {
            arrowRB.velocity = new Vector2(-arrowSpeed, 0f);
            arrow.transform.Rotate(0f, 0f, 90);
        }
    }

    // ---------------------------------------------
    // MELEE PUNCH
    void PunchAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && nextPunchAttack <= Time.time)
        {
            animator.SetTrigger("Punch");
            nextPunchAttack = Time.time + punchAttackSpeed;

            // choose randomly between sounds
            int random = Random.Range(0, 2);
            if (random == 0)
                AudioManager.instance.Play("Player Punch 1");
            else
                AudioManager.instance.Play("Player Punch 2");
        }
    }

    // called in animation frame for punches
    void PunchEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackRadius, 0f, enemyMask);
        for (int i = 0; i < hitEnemies.Length; i++)
        {
            DamageEnemy(hitEnemies[i], stats.damage);

            // gain a little mana for every melee punch hit
            stats.currentMana += stats.manaGainFromAttack;
            manaBar.SetCurrentMana(stats.currentMana);
        }
    }

    // called in animation frame
    void PunchEffect()
    {
        Instantiate(punchEffect, interactor.GetChild(0).transform.position, Quaternion.identity);
    }

    // ---------------------------------------------
    // called in isAttacking.cs behavior script. (shooting left sprites arent included)
    public void RotateLeft()
    {
        // rotate left 
        if (interactor.eulerAngles.z == 270)
            sr.flipX = true;
        else
            sr.flipX = false;
    }

    private void OnDrawGizmosSelected()
    {   
        Gizmos.DrawWireCube(attackPoint.position, attackRadius);
    }
}
