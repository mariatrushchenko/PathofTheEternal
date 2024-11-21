using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public static PlayerPosition instance;

    [Header("Position to be loaded in next scene")]
    public bool correctPositionNextLevel;
    public Vector2 positionForNextScene;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
