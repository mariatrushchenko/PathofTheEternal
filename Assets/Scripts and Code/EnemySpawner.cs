using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("BG Colliders")]
    [SerializeField] List<Collider2D> antiSpawnColliders;

    [Header("Enemy")]
    [SerializeField] GameObject[] enemyPrefab;
    [SerializeField] int maxNumOfEnemies;
    [SerializeField] List<GameObject> enemies;

    [Header("Timer")]
    [SerializeField] float minimumTimerC;
    [SerializeField] float maxTimerC;
    float timer;

    [Header("Coordinates: DO NOT PUT ZERO (STACK OVERFLOW CRASH)")]
    [SerializeField] float minimumX;
    [SerializeField] float maximumX;
    [SerializeField] float minimumY;
    [SerializeField] float maximumY;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] colliderGameObjects = GameObject.FindGameObjectsWithTag("BG Collider");
        for (int i = 0; i < colliderGameObjects.Length; i++)
        {
            antiSpawnColliders.Add(colliderGameObjects[i].GetComponent<Collider2D>());
        }

        timer = Random.Range(minimumTimerC, maxTimerC);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.Count < maxNumOfEnemies)
        {
            if (timer <= 0)
            {
                SpawnEnemyAtLocation();
                timer = Random.Range(minimumTimerC, maxTimerC);
            }
            else
                timer -= Time.deltaTime;
        }

        // delete any empty elements in enemies list
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
                enemies.Remove(enemies[i]);
        }

        // delete any empty elements from collider list
        for (int i = 0; i < antiSpawnColliders.Count; i++)
        {
            if (antiSpawnColliders[i] == null)
                antiSpawnColliders.Remove(antiSpawnColliders[i]);
        }
    }

    void SpawnEnemyAtLocation()
    {
        // find random pos
        Vector2 spawnPos = FindRandomSpawnPosition();

        // check if its valid. If not, repeat cycle using recursion
        bool canSpawnHere = CheckSpawnPosition(spawnPos);
        if (canSpawnHere == true)
        {
            // spawn enemy and add to enemies list
            int i = Random.Range(0, enemyPrefab.Length);
            GameObject enemy = Instantiate(enemyPrefab[i], spawnPos, Quaternion.identity);
            enemies.Add(enemy);
        }
        else
            SpawnEnemyAtLocation();
    }

    bool CheckSpawnPosition(Vector2 spawnPos)
    {
        for (int i = 0; i < antiSpawnColliders.Count; i++)
        {
            Vector3 centerpoint = antiSpawnColliders[i].bounds.center;
            float width = antiSpawnColliders[i].bounds.extents.x;
            float height = antiSpawnColliders[i].bounds.extents.y;

            float leftExtent = centerpoint.x - width;
            float rightExtent = centerpoint.x + width;
            float lowerExtent = centerpoint.y - height;
            float upperExtent = centerpoint.y + height;

            if (spawnPos.x >= leftExtent && spawnPos.x <= rightExtent)
            {
                if (spawnPos.y >= lowerExtent && spawnPos.y <= upperExtent)
                    return false;
            }
        }
        return true;
    }

    Vector2 FindRandomSpawnPosition()
    {
        float randomX = Random.Range(minimumX, maximumX);
        float randomY = Random.Range(minimumY, maximumY);

        Vector2 randomPos = new Vector2(randomX, randomY);
        return randomPos;
    }
}
