using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal
{   
    /* NOTE: if you want to add another quest goal type, do the following:
     * STEP 1: List new goal type in GoalType enum
     * Step 2: Make a new enum type that specifies the types of variations of that new goal type. THEN DECLARE IT AT THE TOP
     * Step 3: Make new function that adds current amount (Just copy and paste EnemyKilled(param) and change the list name
     * Step 4: Go to QuestGiver.cs and where ever you so an if-else for goal type, add the new type
     * Step 5: Go to GameMaster.cs and go to the bottom and add the new type whereever u see an if-else
     */

    // this script is aimed to set the requirements to finish the quest
    // use enumerators to differentiate various quest and other types (for future coding projects, use Inheritence)
    [Header("QUEST TYPE AND REQUIREMENT")]
    public GoalType goalType;
    public int requiredAmount;

    [Header("Single Type Quest? (Non-multivariable)")]
    public bool singleType;
    [HideInInspector] public int currentAmount;

    [Header("Set size of lists as same size as the enum lists. MULTIVARIABLE ONLY")]
    [NonReorderable] 
    public int[] indexCurrentAmount;    
    [NonReorderable] [Tooltip("Make sure the value in each element ADDS UP to the same value of the variable: requiredAmount")]
    public int[] indexMaxAmount;

    [Header("For multi-variable quest requirements")]
    [NonReorderable]
    public EnemyType[] enemyType;
    [NonReorderable]
    public ItemType[] itemType;

    // return T or F whether quest is completed
    public bool IsReached()
    {
        if (singleType == true)
            return currentAmount >= requiredAmount;
        else
            return AddIndexValues();
    }

    /// <summary>
    /// Loop through the indexCurrentAmount list, adding each element's value to an int variable called sum. Then
    /// check if sum is equal to the requiredAmount. If it is, the quest has been completed.
    /// </summary>
    bool AddIndexValues()
    {
        int sum = 0;
        for (int i = 0; i < indexCurrentAmount.Length; i++)        
            sum += indexCurrentAmount[i];
        
        if (sum == requiredAmount)
            return true;
        else
            return false;
    }

    // called this function using: stats.quest.goal.EnemyKilled(). Example is in Treant.cs OnDestroy();
    public void EnemyKilled(string enemyTypeName)
    {       
        if (goalType == GoalType.Kill)
        {   
            // FOR SINGLE TYPE ENEMY KILL
            if (singleType == true)
            {   
                // add to currentAmount and ignore indexCurrentIndex lists
                if (enemyType[0].ToString() == enemyTypeName)              
                    currentAmount++;                 
            }
            else  // MULTIPLE TYPE
            {
                // loop through the enemyType list and find the enemyType index that matches with the enemy name. Once found, 
                // add 1 to the current amount of the correct enemy type/name
                for (int i = 0; i < enemyType.Length; i++)
                {   
                    if (enemyType[i].ToString() == enemyTypeName)
                    {
                        if (indexCurrentAmount[i] < indexMaxAmount[i])
                            indexCurrentAmount[i]++;               
                    }                     
                }
            }
        }
    }

    // called this function using: stats.quest.goal.ItemCollected()
    public void ItemCollected(string itemTypeName)
    {   
        if (goalType == GoalType.Gathering)
        {
            // FOR SINGLE TYPE ITEM COLLECT
            if (singleType == true)
            {
                // add to currentAmount and ignore indexCurrentIndex lists
                if (itemType[0].ToString() == itemTypeName)
                    currentAmount++;
            }
            else  // MULTIPLE TYPE
            {
                // loop through the itemType list and find the itemType index that matches with the item name. Once found, 
                // add 1 to the current amount of the correct item type/name
                for (int i = 0; i < itemType.Length; i++)
                {
                    if (itemType[i].ToString() == itemTypeName)
                    {
                        if (indexCurrentAmount[i] < indexMaxAmount[i])
                            indexCurrentAmount[i]++;
                    }
                }
            }
        }
    }
}

public enum GoalType
{
    Kill,
    Gathering,
}

public enum EnemyType 
{ 
    Treant,
    Mole
}

public enum ItemType
{
    Heart,
    Apple, 
    Gem
}
    

