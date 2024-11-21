using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestGiver : MonoBehaviour
{   
    public List<Quest> questList;

    PlayerMovement pm;

    [SerializeField] Animator dialogueAnim;

    [SerializeField] GameObject textPopup;

    [Header("IMPORTANT: INDEX TO DISPLAY WHICH QUEST")]
    public int questListIndex;

    [Tooltip("Make sure the gameObjects index match with quest index!")]
    // its a prefab so we can save memory
    [SerializeField] GameObject quest2ObjectPrefab;

    [Header("Dialogue")]
    [SerializeField] DialogueTrigger nonActiveQuestDialogue;
    [SerializeField] DialogueTrigger ActiveQuestDialogue;

    [Header("Player Canvas Text")]
    [SerializeField] Text[] questProgressText;

    [Header("Quest Components: LAST ONE IS OPTIONAL")]
    [SerializeField] GameObject questWindow;
    [SerializeField] Text title;
    [SerializeField] Text description;
    [SerializeField] Text goldReward;
    [SerializeField] Image itemRewardImage;

    bool isInRange;

    private void Start()
    {
        nonActiveQuestDialogue = GetComponent<DialogueTrigger>();

        textPopup.SetActive(false);
        DisableQuestProgressText();

        // this is to reset prefs
        //PlayerPrefs.SetInt("DashQuest", 1);
        //PlayerPrefs.SetInt("FirstBoss", 0);
    }

    // Update is called once per frame
    void Update()
    {   
        // OPEN UP THE POPUP
        if (Input.GetKeyDown(KeyCode.Q) && isInRange == true && questWindow.activeSelf == false)
        {   
            if (questList[questListIndex].isActive == false)
            {
                Cursor.visible = true;
                OpenQuestWindow();
                nonActiveQuestDialogue.TriggerDialogue();

                // player cant move
                pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
                pm.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                pm.canMove = false;

                // in the case the player is running while pressing q
                pm.GetComponent<Animator>().SetFloat("Speed", 0);
            }
            else
            {
                // display active quest dialogue
                ActiveQuestDialogue.TriggerDialogue();
            }
        } 
        // CLOSING THE POPUP
        else if (Input.GetKeyDown(KeyCode.Q) && isInRange == true &&
            (questWindow.activeSelf == true || questList[questListIndex].isActive == true))
        {
            Cursor.visible = false;
            CloseQuestWindow();

            // let player move
            pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            pm.canMove = true;
        }

    }

    void OpenQuestWindow()
    {
        // when player completes a specific quest, show a new one
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // player prefs will equal 1 when the FIRST quest is completed (code is in GameMaster) so the second quest will appear.
            // After completing the second quest, RETURN to showing the first (second is unique while first is recurring)
            int firstQuestDone = PlayerPrefs.GetInt("DashQuest", 0);
            if (firstQuestDone == 1)
                questListIndex = 1;
            else
                questListIndex = 0;
        }

        questWindow.SetActive(true);
        title.text = questList[questListIndex].title;
        description.text = questList[questListIndex].description;
        goldReward.text = questList[questListIndex].goldReward.ToString() + " Gold";

        // show the item's sprite in the quest popup
        if (questList[questListIndex].itemReward != null)
        {
            itemRewardImage.sprite = questList[questListIndex].itemReward.GetComponent<SpriteRenderer>().sprite;
            itemRewardImage.SetNativeSize();
        }
        else
            itemRewardImage.gameObject.SetActive(false);

    }

    // this is called on button click
    public void AcceptQuest()
    {
        // make quest progress text visible
        foreach (Text progressText in questProgressText)
            progressText.gameObject.SetActive(true);

        UpdateQuestText();

        // quest is now activated
        questList[questListIndex].isActive = true;

        // if quest contains objects to collected, spawn them it
        if (questListIndex == 1)
            Instantiate(quest2ObjectPrefab, Vector3.zero, Quaternion.identity);

        pm.canMove = true;
        PlayerStats.instance.quest = questList[questListIndex];
        CloseQuestWindow();
    }

    void CloseQuestWindow()
    {   
        questWindow.SetActive(false);
        itemRewardImage.gameObject.SetActive(true);
        dialogueAnim.SetBool("isOpen", false);
    }

    // called in GameMaster as well
    public void UpdateQuestText()
    {   
        if (questList[questListIndex].goal.singleType == true)
        {   
            // only show one line of text
            questProgressText[0].gameObject.SetActive(true);
            questProgressText[1].gameObject.SetActive(false);

            // kill enemy
            if (questList[questListIndex].goal.goalType == GoalType.Kill)
            {
                DisplaySingleTypeText(questList[questListIndex].goal.enemyType[0].ToString());
            }
            // pick up item
            else if (questList[questListIndex].goal.goalType == GoalType.Gathering)
            {
                DisplaySingleTypeText(questList[questListIndex].goal.itemType[0].ToString());
            }
        }
        else
        {
            // display quest progress on Player Canvas
            for (int i = 0; i < questProgressText.Length; i++)
            {   
                // kill enemy
                if (questList[questListIndex].goal.goalType == GoalType.Kill)
                {
                    DisplayMultiTypeText(questList[questListIndex].goal.enemyType[i].ToString(), i);
                }
                // item collect
                else if (questList[questListIndex].goal.goalType == GoalType.Gathering)
                {
                    DisplayMultiTypeText(questList[questListIndex].goal.itemType[i].ToString(), i);
                }
            }
        }
    }
    
    // ------------------------------------------------------------
    // avoid code repetition and improve ease of processing
    void DisplaySingleTypeText(string objectName)
    {   
        questProgressText[0].text = objectName + " | " + +questList[questListIndex].goal.currentAmount
            + "/" + questList[questListIndex].goal.requiredAmount; 
    }

    void DisplayMultiTypeText(string objectName, int i)
    {
        questProgressText[i].text = objectName + " | " + questList[questListIndex].goal.indexCurrentAmount[i] 
            + "/" + questList[questListIndex].goal.indexMaxAmount[i];
    }

    // ------------------------------------------------------------

    // called GameMaster as well
    public void DisableQuestProgressText()
    {
        foreach (Text progressText in questProgressText)
            progressText.gameObject.SetActive(false);

        // reset values for quest
        for (int i = 0; i < questList[questListIndex].goal.indexCurrentAmount.Length; i++)
        {
            questList[questListIndex].goal.indexCurrentAmount[i] = 0;
        }
    }

    // ----------------------------------------------------------
    // trigger functions

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
}
