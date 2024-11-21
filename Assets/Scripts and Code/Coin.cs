using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int coinGainMinimum;
    [SerializeField] int coinGainMax;

    [Header("RENAME FOR EVERY UNIQUE COIN: NO WHITESPACES")]
    [SerializeField] bool uniqueCoin;
    [SerializeField] string playerPrefsName;

    private void Start()
    {   
        if (uniqueCoin == true)
        {
            // to reset
            //PlayerPrefs.SetInt(playerPrefsName, 0);

            if (PlayerPrefs.GetInt(playerPrefsName) == 1)
            {
                Debug.Log("Deleted Coin due to PlayerPrefs");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int randomAmount = Random.Range(coinGainMinimum, coinGainMax + 1);
            PlayerStats.instance.coins += randomAmount;
            AudioManager.instance.Play("Money");

            if (uniqueCoin == true)
                PlayerPrefs.SetInt(playerPrefsName, 1);

            Destroy(gameObject);
        }
    }
}
