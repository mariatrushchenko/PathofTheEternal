using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatsData 
{   
    // add variables here and just copy them into the constructor

    public int levelIndex;

    // lives and health
    public int maxLives;
    public int maxHealth;

    // mana
    public int maxMana;
    public int arrowManaCost;

    // currency
    public int coins;

    // damage
    public int damage;
    public int arrowDamage;

    // movement
    public float runSpeed;
    public bool canDash;

    /// <summary>
    /// Constructor. Pass in PlayerStats Object in scene into parameter and store stats valuess into
    /// the data variables.
    /// </summary>
    /// <param name="stats"></param>
    public PlayerStatsData(PlayerStats stats)
    {
        levelIndex = stats.levelIndex;

        maxLives = stats.maxLives;
        maxHealth = stats.maxHealth;

        maxMana = stats.maxMana;
        arrowManaCost = stats.arrowManaCost;

        coins = stats.coins;

        damage = stats.damage;
        arrowDamage = stats.arrowDamage;

        runSpeed = stats.runSpeed;
        canDash = stats.canDash;
    }
}
