using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; // Can reference this to other scripts 
                                             // Used in Weapon and movement script
    public GameObject pauseMenuUI;
    public GameObject settingsUI;

    Animator pauseAnim;

    private void Start()
    {
        pauseAnim = pauseMenuUI.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused == false)
            {
                // if the game isn't paused, pause the game and set pause menu active
                Pause();
            }
            else
            {
                // if the game is paused and player presses ESC, call the resume function
                Resume();             
            }
        }
    }

    public void Resume()
    {
        // When the player is in settings mode and presses esc, return to pause menu instead of unpausing game
        if (settingsUI.activeSelf == true)
        {
            pauseMenuUI.SetActive(true);
            settingsUI.SetActive(false);
        }
        // Unpause the game if player is in the pause menu
        else
        {
            pauseAnim.enabled = true;
            pauseAnim.SetTrigger("FadeOut");

            Time.timeScale = 1f; // resumes time
            GameIsPaused = false;

            // PLEASE NOTE: Cursor goes invisible when exiting out of pause menu when a quest/shop menu is also open. If people 
            // point that out, type beneath some code that checks if any menu is open (may have to use a For Loop to check NPC and 
            // Quest Popups)
            Cursor.visible = false;
        }
    }

    void Pause()
    {
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // freeze time/game
        GameIsPaused = true;
    }

    public void AnimatorTrue()
    {
        pauseAnim.enabled = true;
    }

    public void AnimatorFalse()
    {
        pauseAnim.enabled = false;
    }

    public void LoadMenu()
    {
        // resume time 
        Time.timeScale = 1f;

        // make transition fade canvas in front of others
        GameObject.FindGameObjectWithTag("TransitionFade").GetComponent<Canvas>().sortingOrder = 10;

        // since time is resumed, destroy player so that player cant move around or die while transitioning
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player);

        // if coroutine doesnt load, make sure scene in loaded in build settings
        StartCoroutine(LevelLoader.instance.LoadLevelByIndex(0));
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
