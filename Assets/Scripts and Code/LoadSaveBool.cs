using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSaveBool : MonoBehaviour
{
    public static LoadSaveBool instance;
    public bool LoadSave;

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
