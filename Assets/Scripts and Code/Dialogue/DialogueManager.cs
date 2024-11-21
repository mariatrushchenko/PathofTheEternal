using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    public Animator animator;

    // Queues are similar to arrays but are more restrictive. Uses a First in First out principle
    // Queue performs the function of a buffer
    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        // create an instance (we have to use NEW because sentences is an empty variable). In simple terms, think about this for Unity,
        // IF THE VARIABLE SHOWS IN THE INSPECTOR, WE DO NOT NEED TO USE NEW BECAUSE WE ARE ALREADY WORKING WITH AN INSTANCE. 
        // (there is no need to make another one)
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // make text box appear
        animator.SetBool("isOpen", true);

        // change default text into given name text
        nameText.text = dialogue.name;

        // clears dialogue box if there was any previous sentences
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {   // queue up the sentences
            sentences.Enqueue(sentence);
        }

        // display first sentence
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // if dialogue finishes
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // get the next sentence
        string sentence = sentences.Dequeue();

        // StopAllCoroutines() stops TypeSentence in the case that the player goes to the next sentence
        // while the sentence is still typing
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        // this code is if you want to display text automatically
        //dialogueText.text = sentence;
    }

    IEnumerator TypeSentence(string sentence)
    {
        // this ienumator is for the text animation of a character one by one

        // make dialogueText text empty
        dialogueText.text = "";

        // turn sentence into an array (characters make up one element each) by using .ToCharArray()
        foreach (char letter in sentence.ToCharArray())
        {
            // add each letter(element) into dialogueText string one by one
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }

    }

    void EndDialogue()
    {
        // close the text box (by moving it back up)
        animator.SetBool("isOpen", false);
    }
}
