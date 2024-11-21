using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsPopup : MonoBehaviour
{
    public static PlayerStatsPopup instance;

    PlayerStats stats;

    [Header("Add more as you show more details")]
    [SerializeField] Text punchDmg;
    [SerializeField] Text arrowDmg;
    [SerializeField] Text maxHealth;
    [SerializeField] Text maxMana;
    [SerializeField] Text arrowCost;
    [SerializeField] Text manaGain;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        stats = PlayerStats.instance;

        // when player loads in
        UpdatePlayerStatsPopupValues(); 
    }

    public void UpdatePlayerStatsPopupValues()
    {
        punchDmg.text = "Punch DMG: " + stats.damage.ToString();
        arrowDmg.text = "Arrow DMG: " + stats.arrowDamage.ToString();
        maxHealth.text = "Max HP: " + stats.maxHealth;
        maxMana.text = "Max Mana: " + stats.maxMana;
        arrowCost.text = "Arrow Cost: " + stats.arrowManaCost;
        manaGain.text = "Mana Gain: " + stats.manaGainFromAttack;
    }
}
