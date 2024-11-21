using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    EnemyKnockback kb;
    Animator animator;
    SlimeMinionSpawn sms;

    bool isFlipped;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Transform player;
    float nextS;

    [SerializeField] int collisionDamage;
    
    [Header("Slime Boss Regular Movement")]
    public float moveSpeed;

    [Header("Slime Dash: DashTimerC called in SlimeBossMove.cs")]
    public Collider2D[] invalidColliders;
    public float dashTimerC;
    [HideInInspector] public float dashTimer;
    public float dashSpeed;
    [SerializeField] float curPosOffset;

    [Header("Increase values below certain health threshold (Set bool to true)")]
    [SerializeField] bool increaseValues;
    [SerializeField] float dashSpeedIncrease;
    Enemy enemy;

    private void Start()    
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponentInParent<Rigidbody2D>();
        kb = GetComponent<EnemyKnockback>();
        animator = GetComponent<Animator>();
        sms = GetComponent<SlimeMinionSpawn>();

        player = GameObject.FindGameObjectWithTag("Player").transform;  
    }

    private void OnDestroy()
    {
        // kill all minions in scene
        if (sms.minionsList.Count > 0)
        {
            for (int i = 0; i < sms.minionsList.Count; i++)
                Destroy(sms.minionsList[i]);  
        }
    }

    private void FixedUpdate()
    {   
        // only knockback when slime is not in dash
        if (kb.knockBackTimer > 0 && animator.GetBool("Dash") == false)
            kb.Knockback(transform.parent.gameObject);
        else if (animator.GetBool("Dash") == false)
            rb.velocity = Vector2.zero;
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        //if current health is less than or equal to half of the max health, increase certain stats.
        // bool is there to avoid doing it twice
        if (increaseValues == true && enemy.enemyStats.currentHealth <= enemy.enemyStats.maxHealth / 2)
        {
            dashSpeed += dashSpeedIncrease;
            increaseValues = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.DamagePlayer(collisionDamage);
            PlayerKnockback.instance.KnockBackPlayer(collision.collider, transform.parent.gameObject);
        }
    }

    // called in SlimeBossMove.cs
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

    void FindPlayer()
    {
        if (nextS <= Time.time)
        {
            GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
            if (searchResult != null)
                player = searchResult.transform;
            nextS = Time.time + 1f;
        }
    }

    // -------------------------------
    // Dash code 

    // called in SlimeBossMove.cs
    /// <summary>
    /// Returns a random offset position from the Slime's transform.position on a particular frame.
    /// </summary>
    public Vector2 GoToOffsetPosition()
    {   
        float xCord = Random.Range(transform.parent.position.x - curPosOffset, transform.parent.position.x + curPosOffset);
        float yCord = Random.Range(transform.parent.position.y - curPosOffset, transform.parent.position.y + curPosOffset);
        Vector2 position = new Vector2(xCord, yCord);

        return position;
    }
}
