using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        // calling this function is up to you. (OntriggerEnter or when player presses a button)

        // call the start dialogue function from Dialogue Manager script using the dialogue written above
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
