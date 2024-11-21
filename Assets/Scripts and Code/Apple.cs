using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    [SerializeField] GameObject itemFeedback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameMaster.gm.CompleteQuest(1, "Apple");

            AudioManager.instance.Play("Item Feedback");
            GameObject effect = Instantiate(itemFeedback, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);

            Destroy(gameObject);
        }
    }
}
