using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectile : MonoBehaviour
{
    Transform player;
    Rigidbody2D rb;

    [SerializeField] int damage;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject effectPrefab;
    [SerializeField] float destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 8);

        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destroyTime);

        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            player = _player.transform;

        // find player vector pos
        Vector2 dir = (player.position - transform.position).normalized * moveSpeed;
        rb.velocity = new Vector2(dir.x, dir.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            Effect(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Player>().DamagePlayer(damage);

            Vector2 spawnPos = collision.collider.ClosestPoint(transform.position);
            Effect(spawnPos);
        }
        else if (collision.collider.CompareTag("BG Collider"))
        {
            Vector2 spawnPos = collision.collider.ClosestPoint(transform.position);
            Effect(spawnPos);
        }
        else if (collision.collider.CompareTag("Arrow"))
        {
            Destroy(collision.collider.gameObject);

            Vector2 spawnPos = collision.collider.ClosestPoint(transform.position);
            Effect(spawnPos);
        }
    }

    void Effect(Vector2 spawnPos)
    {
        GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
        Destroy(effect, 0.5f);

        Destroy(gameObject);
    }
}
