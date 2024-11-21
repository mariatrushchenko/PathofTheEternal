using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    // BUTTON CODE IS AT THE BOTTOM (you have to add a function for onclick for each different shop button)

    public static bool isInShop;

    PlayerStats stats;
    Animator dialogueAnim;
    DialogueTrigger dialogue;
    PlayerMovement pm;
    GameObject nextSentenceText;

    [Header("Popups")]
    [SerializeField] GameObject textPopup;
    [SerializeField] GameObject menuPopup;

    [Header("(3 MAX) TEXT LAYOUT: LEFT -> RIGHT. Edit itemBuyNames string array only")]
    [SerializeField] string[] itemBuyNames;
    [SerializeField] Text[] itemBuyNameTexts;

    [Header("Costs: Edit itemBuyCosts int array only")]
    [SerializeField] int[] itemBuyCosts;
    [SerializeField] Text[] displayCostsText;

    [Header("What the player gains from buying. Edit itemBuyGain float array only")]
    [SerializeField] float[] itemBuyGain;
    [SerializeField] Text[] GainAmountText;
    bool isInRange;

    // Start is called before the first frame update
    void Start()
    {   
        stats = PlayerStats.instance;
        dialogueAnim = FindObjectOfType<DisplayNextSentence>().GetComponent<Animator>();
        dialogue = GetComponent<DialogueTrigger>();

        nextSentenceText = GameObject.FindGameObjectWithTag("ContinueText");

        textPopup.SetActive(false);
        menuPopup.SetActive(false);

        // assign values to texts
        for (int i = 0; i < itemBuyNames.Length; i++)
        {   
            // names texts
            itemBuyNameTexts[i].text = itemBuyNames[i];

            // cost texts
            displayCostsText[i].text = itemBuyCosts[i].ToString();

            // amount to gain text    
            if (itemBuyGain[i] >= 0)
                GainAmountText[i].text = "+" + itemBuyGain[i];
            else
                GainAmountText[i].text = itemBuyGain[i].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // open NPC shop
        if (Input.GetKeyDown(KeyCode.Q) && isInRange == true && menuPopup.activeSelf == false)
        {
            isInShop = true;
            Cursor.visible = true;

            // player cant move while shopping
            pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            pm.canMove = false;
            pm.GetComponent<Animator>().SetFloat("Speed", 0);
            pm.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            // dont show next sentence text
            nextSentenceText.SetActive(false);

            // set popups to true
            GameMaster.gm.playerStatsPopup.SetActive(true);
            menuPopup.SetActive(true);

            // trigger dialogue
            dialogue.TriggerDialogue();
        }  
        // close NPC shop
        else if (Input.GetKeyDown(KeyCode.Q) && isInRange == true && menuPopup.activeSelf == true)
        {
            isInShop = false;
            Cursor.visible = false;

            // let player move
            pm.canMove = true;

            // disable menu and let the next sentence text appear again
            nextSentenceText.SetActive(true);

            GameMaster.gm.playerStatsPopup.SetActive(false);
            menuPopup.SetActive(false);

            // close dialogue box
            dialogueAnim.SetBool("isOpen", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            textPopup.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            textPopup.SetActive(false);
        }
    }

    // ----------------------------
    // BUTTON CODE: REMEMBER TO ADD A FUNCTION FOR EACH DIFFERENT STAT UPGRADE
    // ALSO IF THE STAT IS IN PLAYERSTATSPOPUP, YOU HAVE TO CALLED THE STATIC FUNCTION

    private readonly string StatGain = "StatGain";
    private readonly string Error = "Error";

    // LEVEL 1 SHOP
    public void BuyDamage()
    {
        if (stats.coins >= itemBuyCosts[0])
        {
            stats.coins -= itemBuyCosts[0];
            stats.damage += (int)itemBuyGain[0];

            PlayerStatsPopup.instance.UpdatePlayerStatsPopupValues();
            AudioManager.instance.Play(StatGain);
        }
        else
            AudioManager.instance.Play(Error);
    }

    public void BuyArrowDamage()
    {
        if (stats.coins >= itemBuyCosts[1])
        {
            stats.coins -= itemBuyCosts[1];
            stats.arrowDamage += (int)itemBuyGain[1];

            PlayerStatsPopup.instance.UpdatePlayerStatsPopupValues();
            AudioManager.instance.Play(StatGain);
        }
        else
            AudioManager.instance.Play(Error);
    }

    public void BuyMaxHealth()
    {
        if (stats.coins >= itemBuyCosts[2])
        {

            stats.coins -= itemBuyCosts[2];
            stats.maxHealth += (int)itemBuyGain[2];
            stats.currentHealth = stats.maxHealth;
  
            HealthBar hpBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();
            Text hpText = GameObject.FindGameObjectWithTag("PlayerHealthText").GetComponent<Text>();

            // update hp stuff
            hpBar.SetMaxHealth(stats.maxHealth);
            hpBar.SetCurrentHealth(stats.maxHealth);
            hpText.text = stats.maxHealth.ToString();

            PlayerStatsPopup.instance.UpdatePlayerStatsPopupValues();
            AudioManager.instance.Play(StatGain);
        }
        else
            AudioManager.instance.Play(Error);
    }
    // ---------------------------------
    // NPC HOUSE SHOP
    public void BuyHealthRegenAmt()
    {
        if (stats.coins >= itemBuyCosts[0])
        {
            stats.coins -= itemBuyCosts[0];
            stats.healthRegenAmount += (int)itemBuyGain[0];

            PlayerStatsPopup.instance.UpdatePlayerStatsPopupValues();
            AudioManager.instance.Play(StatGain);
        }
        else
            AudioManager.instance.Play(Error);
    }

    public void BuyManaGainFromAttack()
    {
        if (stats.coins >= itemBuyCosts[1])
        {
            stats.coins -= itemBuyCosts[1];
            stats.manaGainFromAttack += (int)itemBuyGain[1];

            PlayerStatsPopup.instance.UpdatePlayerStatsPopupValues();
            AudioManager.instance.Play(StatGain);
        }
        else
            AudioManager.instance.Play(Error);
    }

    public void BuyArrowManaCostDecrease()
    {
        if (stats.coins >= itemBuyCosts[2] && stats.arrowManaCost > 0)
        {
            stats.coins -= itemBuyCosts[2];
            stats.arrowManaCost += (int)itemBuyGain[2];

            // consider edge case where mana cost for arrow goes negative
            if (stats.arrowManaCost < 0)
                stats.arrowManaCost = 0;

            PlayerStatsPopup.instance.UpdatePlayerStatsPopupValues();
            AudioManager.instance.Play(StatGain);
        }
        else
            AudioManager.instance.Play(Error);
    }

    // ------------------------
}
