using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    PlayerStats stats;

    HealthBar healthBar;
    Text healthText;

    public bool isInvulnerable;

    [Header("Camera Shake")]
    [SerializeField] float camIntensity;
    [SerializeField] float camTime;

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.instance;

        healthBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();
        healthText = GameObject.FindGameObjectWithTag("PlayerHealthText").GetComponent<Text>();

        // initialize HEALTH
        stats.currentHealth = stats.maxHealth;

        // health bar
        healthBar.SetMaxHealth(stats.maxHealth);
        healthBar.SetCurrentHealth(stats.maxHealth);

        // health text
        healthText.text = stats.maxHealth.ToString();

        // allow player regen
        InvokeRepeating(nameof(HealthRegeneration), stats.healthRegenRate, stats.healthRegenRate);
    }

    void HealthRegeneration()
    {   
        // add hp and update health bar and text
        stats.currentHealth += stats.healthRegenAmount;

        healthBar.SetCurrentHealth(stats.currentHealth);
        healthText.text = stats.currentHealth.ToString();
    }

    public void DamagePlayer(int damage)
    {   
        if (isInvulnerable == false)
        {
            stats.currentHealth -= damage;

            // adjust changes to health bar and text
            healthBar.SetCurrentHealth(stats.currentHealth);
            healthText.text = stats.currentHealth.ToString();

            if (stats.currentHealth <= 0)
                GameMaster.gm.KillPlayer(this);
            else
            {
                AudioManager.instance.Play("PlayerHit");
                CinemachineShake.instance.ShakeCamera(camIntensity, camTime);
            }
        }
    }
}
