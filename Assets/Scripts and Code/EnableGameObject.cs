using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    [SerializeField] Collider2D doorOpeningCollider;
    [SerializeField] string playerPrefName;

    private void Update()
    {
        if (PlayerPrefs.GetInt(playerPrefName) == 1)
        {
            doorOpeningCollider.enabled = true;
            Destroy(gameObject);
        }
    }
}
