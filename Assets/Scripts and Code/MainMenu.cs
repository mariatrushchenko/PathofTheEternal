using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    PlayerStats stats;

    [SerializeField] GameObject playBUTTON;
    [SerializeField] GameObject continueBUTTON;
    [SerializeField] GameObject deleteSaveBUTTON;
    [SerializeField] Text buildText;

    private void Start()
    {
        stats = PlayerStats.instance;

        // if you want to reset playerprefs
        //PlayerPrefs.SetInt("ContinueSave", 0);

        // delete play button or continue button (0 is no save, 1 is save)
        int number = PlayerPrefs.GetInt("ContinueSave", 0);
        if (number == 0)
        {
            Destroy(continueBUTTON);
            Destroy(deleteSaveBUTTON);
        }
        else
            Destroy(playBUTTON);

        // show current build
        buildText.text = Application.version;
    }

    public void Play()
    {
        StartCoroutine(LevelLoader.instance.LoadLevelByIndex(1));
    }

    public void ContinueSave()
    {   
        // get saved data and store into player stats variables
        PlayerStatsData data = SaveSystem.LoadPlayerStatsData();
        stats.LoadPlayerStats(data);

        StartCoroutine(LevelLoader.instance.LoadLevelByIndex(data.levelIndex));
        LoadSaveBool.instance.LoadSave = true;
    }

    public void DeleteSave()
    {
        Debug.Log("You deleted the save file!");
        SaveSystem.DeletePlayerFile();
        SaveSystem.DeletePlayerStatsFile();
        PlayerPrefs.DeleteAll();

        Application.Quit();
    }

    public void Quit()
    {
        Debug.Log("You quit the game!");
        Application.Quit();
    }
}
