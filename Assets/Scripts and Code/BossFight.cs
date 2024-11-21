using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossFight : MonoBehaviour
{
    AudioSource audioS;

    [Header("FIGHT BOSS ONLY ONCE: Only list name when bool is true")]
    [SerializeField] bool fightOnlyOnce;
    [SerializeField] string playerPrefsName;
 
    // remember to use arrays for FIXED LENGTH elements and use Lists for DYNAMIC 
    [Header("CINEMACHINE CAMERA")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("OPTIONAL: Boss Audio Track; Attached AudioSource to gameObject if true")]
    public bool hasMusicTrack;
    bool playedMusic;

    // note for triggering boss music: it is called in the ontriggerenter2D method at the very bottom

    [Header("WALLS")]
    public float wallSpeed;
    public Transform[] walls;
    public Transform[] wallGoToPositions;
    public List<Vector3> wallsOriginalPositions;
    bool triggerMove = false;
    bool triggeredOnce = false; // only want to move the walls once

    [Header("ENEMIES")]
    [Tooltip("Some enemies will spawn once player enters area while others will already be active")]
    public bool spawnEnemies;  
    public List<GameObject> enemies;

    [Header("Item Reward Spawn: THIS IS SPAWNED ON TRANSFORM.POSITION")]
    [SerializeField] bool spawnItemAfterFight;
    [SerializeField] GameObject itemPrefab;

    // called when fight is finished
    bool isDone;

    [Header("Item Spawner")]
    [SerializeField] GameObject[] itemSpawners;

    // Start is called before the first frame update
    void Start()
    {   
        // get original positions
        for (int i = 0; i < walls.Length; i++)
            wallsOriginalPositions.Add(walls[i].position);

        if (hasMusicTrack)
            audioS = GetComponent<AudioSource>();

        // to set playerprefs
        //PlayerPrefs.SetInt(playerPrefsName, 0);

        // if bool is toggled, no need for this script and gameobject (walls, transforms, etc will be destroyed too
        // as it will be parented to the gameobject that is to be destroyed)
        // NOTE: toggle fightOnlyOnce as false to spawn Boss for testing purposes)
        if (fightOnlyOnce == true && PlayerPrefs.GetInt(playerPrefsName) == 1)
        {
            Destroy(gameObject);

            foreach (GameObject enemy in enemies)
                Destroy(enemy);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // move walls and spawn enemies if possible
        if (triggerMove == true && triggeredOnce == false)       
            BeginFight();
        

        // move walls back once player has cleared all enemies
        if (enemies.Count == 0 && isDone == false)
        {   
            // stop music, move back walls, spawn item, etc
            EndFight();

            // if we only want to fight once, dont spawn next time player enters room
            if (fightOnlyOnce == true)
                PlayerPrefs.SetInt(playerPrefsName, 1);
        }
        else if (enemies.Count != 0)
        {
            // delete element when enemy is killed
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                    enemies.Remove(enemies[i]);
            }
        }
    }

    void BeginFight()
    {
        //activate camera
        virtualCamera.m_Priority = 99;

        // true by default
        triggeredOnce = true;

        // move walls
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].position = Vector2.MoveTowards(walls[i].position, wallGoToPositions[i].position, wallSpeed * Time.deltaTime);

            // check if walls are at their designated location
            if (walls[i].position != wallGoToPositions[i].position)
                triggeredOnce = false;
        }

        // spawn enemies if toggled
        if (spawnEnemies == true)
        {
            for (int i = 0; i < enemies.Count; i++)
                enemies[i].SetActive(true);
        }
    }

    void EndFight()
    {
        // if there is a music track, turn it off once boss is dead
        if (hasMusicTrack == true)
        {
            audioS.Stop();
            hasMusicTrack = false;
        }

        // spawn item if there is any
        if (spawnItemAfterFight == true)
        {
            Instantiate(itemPrefab, transform.position, transform.rotation);
            spawnItemAfterFight = false;
        }

        // destroy any item spawners
        if (itemSpawners.Length > 0)
        {
            for (int i = 0; i < itemSpawners.Length; i++)
                Destroy(itemSpawners[i]);
        }

        // bool will be true by default
        isDone = true;

        // fight is done once the walls are back in their original pos
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].position = Vector2.MoveTowards(walls[i].position, wallsOriginalPositions[i], wallSpeed * Time.deltaTime);

            // check if walls are back in their pos
            if (walls[i].position != wallsOriginalPositions[i])
                isDone = false;
        }

        // deactivate camera
        virtualCamera.m_Priority = 2;

        if (isDone == true)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            triggerMove = true;

            // play special audio boss track if enabled
            if (hasMusicTrack == true && playedMusic == false)
            {
                audioS.Play();
                
                // prevent from triggering the audio again
                playedMusic = true;
            }

            // spawn item spawners if there are any
            if (itemSpawners.Length > 0)
            {
                for (int i = 0; i < itemSpawners.Length; i++)
                    itemSpawners[i].SetActive(true);
            }
        }
    }
}
