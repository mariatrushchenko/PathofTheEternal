using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void Quit()
    {
        Debug.Log("You quit the game!");
        Application.Quit();
    }

    public void LoadMenu()
    {
        StartCoroutine(LevelLoader.instance.LoadLevelByIndex(0));
    }

    public void MouseOver()
    {
        AudioManager.instance.Play("MouseOver");
    }

    public void MouseClick()
    {
        AudioManager.instance.Play("MouseClick");
    }
}
