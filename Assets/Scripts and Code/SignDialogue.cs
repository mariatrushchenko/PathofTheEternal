using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignDialogue : MonoBehaviour
{
    Animator dialogueAnimator;
    [SerializeField] GameObject popupText;
    bool isInRange;

    // Start is called before the first frame update
    void Start()
    {
        dialogueAnimator = FindObjectOfType<DisplayNextSentence>().GetComponent<Animator>();
        popupText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isInRange == true)
            GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            popupText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            popupText.SetActive(false);

            dialogueAnimator.SetBool("isOpen", false);
        }
    }
}
