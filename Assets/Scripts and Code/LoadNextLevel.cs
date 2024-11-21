using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{   
    [Header("Default settings loads next level index in build")]
    [SerializeField] GameObject textUI;
    bool isInRange;
    bool activatedOnce;

    [Header("Specific Player Spawn Next Lvl")]
    [SerializeField] bool specificPlayerSpawn;
    [SerializeField] Vector2 playerSpawnPos;

    [Header("OPTIONAL: (MUST DO IF SPECIFIC SPAWN IS TOGGLED)")]
    [SerializeField] bool loadByString;
    [SerializeField] string levelName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isInRange == true && activatedOnce == false)
        {   
            // avoid possibility of player spamming
            activatedOnce = true;

            RemovePowerUpBuffs();

            // load player in the right spawn next scene 
            if (specificPlayerSpawn == true)
            {
                PlayerPosition playerPos = PlayerPosition.instance;
                playerPos.correctPositionNextLevel = true;
                playerPos.positionForNextScene = playerSpawnPos;
            }

            // load by string or by index
            if (loadByString == true)
                StartCoroutine(LevelLoader.instance.LoadLevelByString(levelName));
            else
            {
                int nextLevelindex = SceneManager.GetActiveScene().buildIndex + 1;
                StartCoroutine(LevelLoader.instance.LoadLevelByIndex(nextLevelindex)); 
            }
        }
    }

    void RemovePowerUpBuffs()
    {
        PlayerStats stats = PlayerStats.instance;

        // Adjust values if a powerup is active
        if (PowerUp.movementPowerUp == true)
        {
            PowerUp.movementPowerUp = false;
            stats.runSpeed -= PowerUp.movementValue;
        }

        if (PowerUp.punchDamagePowerUp == true)
        {
            PowerUp.punchDamagePowerUp = false;
            stats.damage -= (int)PowerUp.punchDamageValue;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            textUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            textUI.SetActive(false);
        }
    }
}
