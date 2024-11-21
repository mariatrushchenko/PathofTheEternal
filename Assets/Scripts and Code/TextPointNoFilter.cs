using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPointNoFilter : MonoBehaviour
{
    [SerializeField] Font[] font;

    // Start is called before the first frame update
    void Start()
    {   
        // pixel perfect
        for (int i = 0; i < font.Length; i++)
            font[i].material.mainTexture.filterMode = FilterMode.Point;
    }
}
