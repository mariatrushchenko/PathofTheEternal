using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{   
    public void Restart()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LevelLoader.instance.LoadLevelByIndex(currentLevelIndex));
        LoadSaveBool.instance.LoadSave = true;
    }

    public void Quit()
    {
        Debug.Log("You quit the game!");
        Application.Quit();
    }
}
