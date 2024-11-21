using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMinion : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyKnockback kb;

    [SerializeField] int collisionDamage;

    [Header("Speed")]
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    float speed;

    [Header("If the object is facing right, toggle isFlipped to true")]
    public bool isFlipped;

    Transform player;
    float nextSearch;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        kb = GetComponent<EnemyKnockback>();

        // cache to avoid error where player dies and then slime spawns
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            player = _player.transform;

        // randomize speed
        speed = Random.Range(minSpeed, maxSpeed);
    }

    private void FixedUpdate()
    {
        if (kb.knockBackTimer > 0)
            kb.Knockback(transform.parent.gameObject);
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
            rb.velocity = Vector2.zero;
            transform.parent.position = Vector2.MoveTowards(transform.parent.position, player.position, speed * Time.deltaTime);
            FacePlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player != null)
        {
            player.DamagePlayer(collisionDamage);

            // instead of usually knocking back the player, knockback the slime instead
            kb.knockBackTimer = kb.knockBackTimerCountdown;
            kb.knockBackSourcePosition = player.transform.position;
        }
    }

    void FacePlayer()
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
        if (nextSearch <= Time.time)
        {
            GameObject s = GameObject.FindGameObjectWithTag("Player");
            if (s != null)
                player = s.transform;
            nextSearch = Time.time + 1f;
        }
    }
}
