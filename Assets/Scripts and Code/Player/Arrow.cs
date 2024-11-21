using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    PlayerStats stats;
    [SerializeField] GameObject particlePrefab;
    [SerializeField] float destroyTime;

    private void Start()
    {
        stats = PlayerStats.instance;
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>().DamageEnemy(stats.arrowDamage);
            Effect(collision.collider);
        }
        else if (collision.collider.CompareTag("BG Collider"))
            Effect(collision.collider);
    }

    void Effect(Collider2D collision)
    {
        AudioManager.instance.Play("Arrow Hit");

        Vector2 particleSpawnPos = collision.ClosestPoint(transform.position);
        GameObject effect = Instantiate(particlePrefab, particleSpawnPos, Quaternion.identity);
        Destroy(effect, 0.5f);

        Destroy(gameObject);
    }
}
