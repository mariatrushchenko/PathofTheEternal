using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMinionSpawn : MonoBehaviour
{
    [SerializeField] GameObject minionPrefab;
    [SerializeField] int maxAmountOfMinions;
    public List<GameObject> minionsList;

    [Header("Coordinate Range")]
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;

    [Header("Timer")]
    [SerializeField] float minTimerC;
    [SerializeField] float maxTimerC;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        float randomTimerC = Random.Range(minTimerC, maxTimerC);
        timer = randomTimerC;
    }

    // Update is called once per frame
    void Update()
    {
        // delete any empty elements
        for (int i = 0; i < minionsList.Count; i++)
        {
            if (minionsList[i] == null)
                minionsList.Remove(minionsList[i]);
        }

        // only run code when max # of minions not met
        if (minionsList.Count < maxAmountOfMinions)
        {
            if (timer <= 0)
            {
                SpawnMinion();
                timer = RandomTimer();
            }
            else
                timer -= Time.deltaTime;
        }
    }

    // spawn minion at random position and add it to enemies list
    void SpawnMinion()
    {
        float cordX = Random.Range(minX, maxX);
        float cordY = Random.Range(minY, maxY);
        Vector2 spawnPosition = new Vector2(cordX, cordY);

        GameObject enemy = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
        minionsList.Add(enemy);
    }

    float RandomTimer()
    {
        float randomTimerC = Random.Range(minTimerC, maxTimerC);
        return randomTimerC;
    }
}
