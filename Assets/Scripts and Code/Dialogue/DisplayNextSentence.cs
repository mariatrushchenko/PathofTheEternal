using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayNextSentence : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && NPC.isInShop == false)
            FindObjectOfType<DialogueManager>().DisplayNextSentence();
    }
}
