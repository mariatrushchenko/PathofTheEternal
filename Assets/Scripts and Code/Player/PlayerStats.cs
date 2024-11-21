using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{   
    // IMPORTANT THING TO NOTW: For future saves when there is an introduction of power ups, have a bool check if a player has
    // a power up. Maybe subtract the power up gain and save that value

    // ADDING VARIABLES - QUICK REFRESHER
    // STEP 1: Add variable in this script
    // STEP 2: In LoadPlayerStats, code should be like: variable = data.variable;
    // STEP 3: In PlayerStatsData.cs, add variable there
    // STEP 4: In PlayerStatsData.cs in the constructor, could should be: variable = stats.variable;

    public static PlayerStats instance;

    [Header("Level Index")]
    public int levelIndex;

    [Header("Quests")]
    public Quest quest;

    [Header("Currency")]
    public int coins;

    [Header("Attack")]
    public int damage;
    public int arrowDamage;

    [Header("Health/Mana Regeneration:")]
    public int healthRegenAmount;
    public float healthRegenRate;
    public int manaGainFromAttack;

    [Header("Lives and Max Health")]
    public int currentLives;
    public int maxLives;
    public int maxHealth;

    [Header("Mana and Mana Costs")]
    public int maxMana;
    public int arrowManaCost;

    [Header("Movement Values")]
    public float runSpeed;
    public bool canDash;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded()
    {   
        // reset lives w/e player reaches new lvl
        currentLives = maxLives;
    }

    /// <summary>
    /// CALLED IN MENU SCREEN. 
    /// NOTE: Make sure to add to this when you progress in the project! In addition, add code to the PlayerStatsData.cs file too.
    /// </summary>
    /// <param name="data"></param>
    public void LoadPlayerStats(PlayerStatsData data)
    {   
        // level index
        levelIndex = data.levelIndex;

        // currency
        coins = data.coins;

        // lives and health
        maxLives = data.maxLives;
        maxHealth = data.maxHealth;

        // mana
        maxMana = data.maxMana;
        arrowManaCost = data.arrowManaCost;

        // damage
        damage = data.damage;
        arrowDamage = data.arrowDamage;

        runSpeed = data.runSpeed;
        canDash = data.canDash;
    }


    [Header("Dont Change This! Read Only")]
    // THING TO NOTE: If you wanna see or change values, you have to inspect _currentHealth and _currentMana. The other public ints are
    // used as getters and setters so the actual values live in the underscored variables

    // health
    [SerializeField] int _currentHealth;
    public int currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    // mana
    [SerializeField] int _currentMana;
    public int currentMana
    {
        get { return _currentMana; }
        set { _currentMana = Mathf.Clamp(value, 0, maxMana); }
    }
}
