using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    /* VERY IMPORTANT: Make sure everytime you add another powerup type (enum), you go to SaveSystem.cs and look for
     * the SavePlayerStats static function in addition to adding code in the PowerUpBuff() Ienumerator in this script. 
     * This is crucial because we want to avoid saving a powerup stats value.
     * */

    // bools and floats are referenced in SaveSystem.cs in SavePlayerStats;
    public static bool movementPowerUp;
    public static bool punchDamagePowerUp;

    public static float movementValue;
    public static float punchDamageValue;

    [SerializeField] float waitTime;
    [SerializeField] GameObject itemFeedback;

    [Header("Works for both int and float (flexible between PlayerStats data types")]
    public PowerUpType powerUpType;
    public float amount;

    PlayerStats stats;

    private void Start()
    {
        // reference stats
        stats = PlayerStats.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && movementPowerUp == false && punchDamagePowerUp == false)
        {
            PowerUpBuff();
        }
    }

    void PowerUpBuff()
    {
        AudioManager.instance.Play("Item Feedback");
        GameObject effect = Instantiate(itemFeedback, transform.position, transform.rotation);
        Destroy(effect, .5f);

        // add to stats
        AddOrSubtractPowerUp(stats, amount, true);

        // disable spriterenderer and collider (object only gets destroyed after waitTime)
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        Invoke(nameof(SubtractPowerUpStats), waitTime);
    }

    void SubtractPowerUpStats()
    {
        // subtract from stats
        if (movementPowerUp == true || punchDamagePowerUp == true)
            AddOrSubtractPowerUp(stats, -amount, false);

        Destroy(gameObject);
    }

    // NOTE: Insert negative values for when you want to subtract the amount from stats variables
    void AddOrSubtractPowerUp(PlayerStats stats, float amount, bool isActive)
    {
        if (powerUpType == PowerUpType.RunSpeed)
        {
            stats.runSpeed += amount;

            movementPowerUp = isActive;
            movementValue = amount;
        }
        else if (powerUpType == PowerUpType.PunchDamage)
        {
            stats.damage += (int)amount;

            punchDamagePowerUp = isActive;
            punchDamageValue = amount;
        }
    }

    public enum PowerUpType
    {
        RunSpeed,
        PunchDamage
    }
}
