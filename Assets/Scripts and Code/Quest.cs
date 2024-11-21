using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Quest
{   
    [Header("Initial Quest Variables")]
    public bool isActive;

    public string title;

    [TextArea(3, 10)]
    public string description;

    public int goldReward;

    [Header("OPTIONAL")]
    public GameObject itemReward;

    [Header("Quest Goal")]
    public QuestGoal goal;

    public void Complete()
    {
        isActive = false;
        goal.currentAmount = 0;
        AudioManager.instance.Play("Quest Complete");
        Debug.Log(title + " was completed!");
    }
}
