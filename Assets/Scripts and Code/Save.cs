using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Save : MonoBehaviour
{
    PlayerStats stats;
    bool isInRange;

    [SerializeField] GameObject tooltipTextUI;
    [SerializeField] GameObject saveText;

    private void Start()
    {
        tooltipTextUI.SetActive(false);

        stats = PlayerStats.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isInRange == true)
        {
            saveText.SetActive(true);

            // replenish health
            HealthBar healthBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();
            Text healthText = GameObject.FindGameObjectWithTag("PlayerHealthText").GetComponent<Text>();

            stats.currentHealth = stats.maxHealth;
            healthBar.SetCurrentHealth(stats.currentHealth);
            healthText.text = stats.currentHealth.ToString();

            // save data
            SaveSystem.SavePlayerData(FindObjectOfType<Player>());
            SaveSystem.SavePlayerStatsData(FindObjectOfType<PlayerStats>());

            // show continue button in menu for now on
            PlayerPrefs.SetInt("ContinueSave", 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            tooltipTextUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            tooltipTextUI.SetActive(false);
        }
    }
}
