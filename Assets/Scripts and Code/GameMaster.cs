using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{   
    // IF YOU ARE LOOKING FOR QUEST COMPLETE CODE, IT IS AT THE VERY BOTTOM!

    public static GameMaster gm;

    PlayerStats stats;

    [Header("Quest Complete Text")]
    public GameObject questCompleteText;

    [Header("Cinemachine Cameras")]
    [SerializeField] List<CinemachineVirtualCamera> cinemachineCameras;

    [Header("PlayerStats Popup")]
    public GameObject playerStatsPopup;

    [Header("Player Components")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float playerRespawnDelay;
    public Vector2 checkpointSpawnPosition;

    [Header("CinemachineShake Values")]
    [SerializeField] float camIntensity;
    [SerializeField] float camTime;

    [Header("Enemy Death Particles")]
    [SerializeField] GameObject enemiesDeathPrefab;

    [Header("Game Over UI")]
    [SerializeField] GameObject gameOverUI;

    [Header("Coin Gain")]
    public int coinGain;
    [SerializeField] float coinGainDelay;

    private void Awake()
    {
        if (gm == null)
            gm = this;
    }

    private void OnLevelWasLoaded()
    {   
        // Load save if bool is true (the bool is turned false in start method)
        if (LoadSaveBool.instance.LoadSave == true)
        {
            PlayerData data = SaveSystem.LoadPlayerData();
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;

            Vector3 savedPosition;
            savedPosition.x = data.position[0];
            savedPosition.y = data.position[1];
            savedPosition.z = data.position[2];

            player.position = savedPosition;
        } 
    }

    private void Start()
    {
        stats = PlayerStats.instance;
        stats.levelIndex = SceneManager.GetActiveScene().buildIndex;

        // make cursor invisible initially
        Cursor.visible = false;

        // if save bool is true, make it false. If it is already false, instant save (player first time playing or transitioning levels)
        // (this is used for restart purposes when player loses game)
        if (LoadSaveBool.instance.LoadSave == true)
        {
            Debug.Log("Save Bool set to false.");
            LoadSaveBool.instance.LoadSave = false;
        }
        else
        {
            // auto save when player enters a level 
            Debug.Log("Autosaved");
            SaveSystem.SavePlayerData(FindObjectOfType<Player>());
            SaveSystem.SavePlayerStatsData(FindObjectOfType<PlayerStats>());
            PlayerPrefs.SetInt("ContinueSave", 1);
        }

        // SPAWN PLAYER CORRECTLY BETWEEN SCENES
        if (PlayerPosition.instance.correctPositionNextLevel == true)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.position = PlayerPosition.instance.positionForNextScene;
            PlayerPosition.instance.correctPositionNextLevel = false;
        }

        // TESTING PURPOSES ONLY
        //DeleteSaveFiles();

        // add cinemachine cameras to list
        CinemachineVirtualCamera[] cameras = FindObjectsOfType<CinemachineVirtualCamera>();
        for (int i = 0; i < cameras.Length; i++)
            cinemachineCameras.Add(cameras[i]);
    }

    // MAKE SURE TO COMMENT OUT THE AUTO SAVE CODE ABOVE WHEN TESTING
    void DeleteSaveFiles()
    {
        SaveSystem.DeletePlayerFile();
        SaveSystem.DeletePlayerStatsFile();
        PlayerPrefs.SetInt("ContinueSave", 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && NPC.isInShop == false)
            playerStatsPopup.SetActive(!playerStatsPopup.activeSelf);        
    }

    // ------------------------------------
    // Player

    public void KillPlayer(Player player)
    {
        AudioManager.instance.Play("Player Death");
        player.GetComponent<Animator>().SetBool("isDead", true);
        DestroyPlayerComponents(player);

        // do quick camera shake before resetting all intensity and frequency values (avoid cam shake bugs)
        CinemachineShake.instance.ShakeCamera(camIntensity, camTime);
        Invoke(nameof(ResetCameraValues), 0.5f);

        // subtract lives
        stats.currentLives -= 1;
        if (stats.currentLives <= 0)
        {   
            Destroy(player.gameObject); // player is usually supposed to play animation and is only destroyed when respawning
            AudioManager.instance.Play("Game Over");
            gameOverUI.SetActive(true);
        }
        else
            StartCoroutine(RespawnPlayer());
    }

    void DestroyPlayerComponents(Player player)
    {
        Destroy(player.GetComponent<Collider2D>());
        Destroy(player.GetComponent<Player>());
        Destroy(player.GetComponent<PlayerMovement>());
        Destroy(player.GetComponent<Rigidbody2D>());
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(playerRespawnDelay);

        // destroy player
        GameObject playerInScene = GameObject.FindGameObjectWithTag("Player");
        if (playerInScene != null)
            Destroy(playerInScene);

        // spawn player at checkpoint
        GameObject newPlayer = Instantiate(playerPrefab, checkpointSpawnPosition, Quaternion.identity);

        // make cameras follow new instantiated player object
        for (int i = 0; i < cinemachineCameras.Count; i++)
            cinemachineCameras[i].m_Follow = newPlayer.transform;
    }

    void ResetCameraValues()
    {
        foreach (CinemachineVirtualCamera cam in cinemachineCameras)
            CinemachineShake.instance.ResetValues(cam);
    }

    // -----------------------------------
    // Enemy

    public void KillEnemy(Enemy enemy)
    {   
        if (coinGain > 0)
        {
            // gain money
            StartCoroutine(nameof(GainCoin));
        }

        // Note: This code below was originally place in OnDestroy() in Enemy.cs but editor kept producing errors whenever exiting editor play
        // check if enemy is part of an active quest goal
        if (enemy.questEnemy == true)
           CompleteQuest(0, enemy.typeOfEnemyName);

        // death effects
        AudioManager.instance.Play("Enemy Death");
        GameObject effect = Instantiate(enemiesDeathPrefab, enemy.transform.position, Quaternion.identity);
        Destroy(effect, 0.5f);

        Destroy(enemy.gameObject);
    }

    // want to have a slight delay because we dont want to overlap enemy death sound with coin sound
    public IEnumerator GainCoin()
    {
        yield return new WaitForSeconds(coinGainDelay);
        stats.coins += coinGain;
        AudioManager.instance.Play("Money");
    }

    // ----------------------------------
    public bool FindParameter(string paramName, Animator animator)
    {
        // loop through each parameter in the animator parameter list
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (paramName == parameter.name)
                return true;

        return false;
    }

    // -----------------------------
    // QUEST

    /// <summary>
    /// For the 1st parameter, input 0 for ENEMY KILL and 1 for ITEM COLLECT. For the 2nd parameter,
    /// input the type of item and enemy using system (Example: For a kill quest, input Treant to check if we killed a Treant)
    /// </summary>
    /// <param name="enemyOrItem"></param>
    public void CompleteQuest(int enemyOrItem, string enemyOrItemNAME)
    {   
        // check if there is a quest (quest goes null upon completion)
        if (stats.quest != null)
        {
            // check if quest is active
            if (stats.quest.isActive == true)
            {
                CheckIfEnemyOrItem(enemyOrItem, enemyOrItemNAME);

                // if we killed enough enemies
                if (stats.quest.goal.IsReached())
                {   
                    RewardPlayer();

                    // some quests will trigger unique ones
                    SpecificQuestTriggers();

                    // disable quest text on player canvas
                    FindObjectOfType<QuestGiver>().DisableQuestProgressText();

                    // mark quest as inactive and set quest to null to reset
                    stats.quest.Complete();
                    stats.quest = null;
                }
            }
        }
    }

    void SpecificQuestTriggers()
    {   
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {   
            // When player completes quest 1, let them have access to quest 2. After completing quest 2, go back to quest 1
            // (quest 2 is unique so it cant be repeated)

            string questName1 = "Kill Those Dirty Scumbags!";
            string questName2 = "They Took Our Items!";

            if (stats.quest.title == questName1 && PlayerPrefs.GetInt("DashQuest") != 3)
                PlayerPrefs.SetInt("DashQuest", 1);
            else if (stats.quest.title == questName2)
                PlayerPrefs.SetInt("DashQuest", 3);
        }
    }

    void CheckIfEnemyOrItem(int enemyOrItem, string enemyOrItemNAME)
    {
        if (enemyOrItem == 0)
        {
            // kill enemy function is called (adds to current amount)
            stats.quest.goal.EnemyKilled(enemyOrItemNAME);
        }
        else if (enemyOrItem == 1)
        {
            // collect item function is called (adds to current amt)
            stats.quest.goal.ItemCollected(enemyOrItemNAME);
        }
        else
        {
            // display error if any int other than the given
            Debug.LogError("Parameter has to be in the given range!", gameObject);
        }

        // update quest progress text
        FindObjectOfType<QuestGiver>().UpdateQuestText();
    }

    void RewardPlayer()
    {
        // add coins 
        stats.coins += stats.quest.goldReward;

        // display completion of the particular quest
        questCompleteText.SetActive(true);
        questCompleteText.GetComponent<Text>().text = "Quest has been completed!";

        // IF there is an item, spawn it on top of the player (easier than having them go back and collect it)
        if (stats.quest.itemReward != null)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            Instantiate(stats.quest.itemReward, player.position, Quaternion.identity);
        }
    }
}
