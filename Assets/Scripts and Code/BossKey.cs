using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKey : MonoBehaviour
{
    [SerializeField] string playerPrefName;
    [SerializeField] GameObject itemFeedback;

    DialogueTrigger trigger;

    private void Start()
    {
        trigger = GetComponent<DialogueTrigger>();
        Invoke(nameof(TriggerTrue), 1f);
    }

    void TriggerTrue()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPrefs.SetInt(playerPrefName, 1);
            trigger.TriggerDialogue();

            AudioManager.instance.Play("Item Feedback");
            GameObject effect = Instantiate(itemFeedback, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);

            Destroy(gameObject);
        }
    }
}
