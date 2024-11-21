using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPotion : MonoBehaviour
{
    PlayerStats stats;
    HealthBar playerHealthBar;
    Text healthText;

    [SerializeField] GameObject itemFeedback;

    [Header("Health")]
    public int healthGain;

    [Header("Lives")]
    public int livesGained;
    public bool gainMaxLivesWithCurrent;

    [Header("Only name if heart.maxLivesGained is true")]
    [SerializeField] string playerPrefsName;

    // Start is called before the first frame update
    void Start()
    {
        stats = PlayerStats.instance;

        if (gainMaxLivesWithCurrent == true)
        {
            int num = PlayerPrefs.GetInt(playerPrefsName, 0);
            if (num == 1)
            {
                Destroy(gameObject);
                Debug.Log("Destroyed: " + gameObject.name + " because of PlayerPrefs.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealthBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();
            healthText = GameObject.FindGameObjectWithTag("PlayerHealthText").GetComponent<Text>();

            // add health
            stats.currentHealth += healthGain;

            // update hp bar and text
            playerHealthBar.SetCurrentHealth(stats.currentHealth);
            healthText.text = stats.currentHealth.ToString();

            // add current lives
            if (livesGained != 0)
                stats.currentLives += livesGained;

            // add livesGained to max lives as well
            if (gainMaxLivesWithCurrent == true)
            {
                stats.maxLives += livesGained;
                PlayerPrefs.SetInt(playerPrefsName, 1);
                SaveSystem.SavePlayerStatsData(FindObjectOfType<PlayerStats>());
            }

            Effect();
        }
    }

    void Effect()
    {
        AudioManager.instance.Play("Item Feedback");
        GameObject effect = Instantiate(itemFeedback, transform.position, Quaternion.identity);
        Destroy(effect, 0.5f);

        Destroy(gameObject);
    }
}
