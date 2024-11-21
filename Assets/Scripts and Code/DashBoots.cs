using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBoots : MonoBehaviour
{
    [SerializeField] GameObject itemFeedback;
    DialogueTrigger dialogue;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = GetComponent<DialogueTrigger>();

        if (PlayerStats.instance.canDash == true)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats.instance.canDash = true;
            dialogue.TriggerDialogue();
            SaveSystem.SavePlayerStatsData(FindObjectOfType<PlayerStats>());

            AudioManager.instance.Play("Item Feedback");
            GameObject effect = Instantiate(itemFeedback, transform.position, transform.rotation);
            Destroy(effect, 0.5f);

            Destroy(gameObject);
        }
    }
}
