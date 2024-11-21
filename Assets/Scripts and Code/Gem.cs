using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] GameObject itemFeedback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameMaster.gm.CompleteQuest(1, "Gem");

            AudioManager.instance.Play("Item Feedback");
            GameObject feedback = Instantiate(itemFeedback, transform.position, Quaternion.identity);
            Destroy(feedback, 0.5f);

            Destroy(gameObject);
        }
    }
}
