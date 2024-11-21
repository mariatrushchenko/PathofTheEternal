using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] GameObject collisionParticleEffect;

    [SerializeField] float rotateSpeed;

    [SerializeField] string[] NonPlayerTags;

    [Header("Toggle if Collision Effects spawn at ClosestPoint")]
    [SerializeField] bool effectsAtClosestPoint;

    [Header("Timer if object doesn't hit anything")]
    [SerializeField] float timerCountdown;
    float timer;

    Transform target;
    Rigidbody2D rb;
    Collider2D thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<Collider2D>();

        timer = timerCountdown;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
        else
            DestroyAndSpawnEffects();
    }

    private void FixedUpdate()
    {
        // destroy player
        if (target == null)
        {
            DestroyAndSpawnEffects();
            return;
        }

        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        // get cross produce to help rotate sprite correctly
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;
    }

    private void Update()
    {
        if (timer <= 0)
            DestroyAndSpawnEffects();
        else
            timer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().DamagePlayer(damage);
            CinemachineShake.instance.ShakeCamera(10f, 0.1f);

            CreateEffect(collision);
            Destroy(gameObject);
        }

        if (NonPlayerTags.Contains(collision.tag))
        {
            CreateEffect(collision);
            Destroy(gameObject);
        }
    }

    // use this function for everything EXCEPT OnTriggerEnter;
    void DestroyAndSpawnEffects()
    {
        effectsAtClosestPoint = false;
        CreateEffect(thisCollider);
        Destroy(gameObject);
    }

    void CreateEffect(Collider2D collision)
    {
        Vector3 hitPos;

        // spawn at closest point or at transform.position
        if (effectsAtClosestPoint == true)
            hitPos = collision.ClosestPoint(transform.position);
        else
            hitPos = transform.position;

        GameObject effect = Instantiate(collisionParticleEffect, hitPos, transform.rotation);
        Destroy(effect, 1f);
    }
}
