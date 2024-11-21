using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> itemList;

    [SerializeField] GameObject itemPrefab;

    [Header("Timer to spawn item")]
    [SerializeField] float timerC;
    float timer;

    [Header("TOGGLE IF COORDINATE SPAWN")]
    [SerializeField] bool coordinateSpawn;
    [SerializeField] float minX, maxX, minY, maxY;

    // Start is called before the first frame update
    void Start()
    {
        timer = timerC;
    }

    // Update is called once per frame
    void Update()
    {
        // delete any empty elements
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == null)
                itemList.Remove(itemList[i]);
        }

        // check if max amount isnt reached yet
        if (itemList.Count < 1)
        {
            if (timer <= 0)
            {
                SpawnItem();
                timer = timerC;
            }
            else
                timer -= Time.deltaTime;
        }
    }

    void SpawnItem()
    {   
        // spawn at coordinates
        if (coordinateSpawn == true)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector2 position = new Vector2(randomX, randomY);

            GameObject item = Instantiate(itemPrefab, position, transform.rotation);
            itemList.Add(item);
        }
        else
        {
            // instantiate item at TRANSFORM.POSITION and add object to list
            GameObject item = Instantiate(itemPrefab, transform.position, transform.rotation);
            itemList.Add(item);
        }
    }
}
