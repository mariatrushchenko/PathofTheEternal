using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenPatrol : MonoBehaviour
{
    public float moveSpeed;
    public Vector3[] waypoints;
    public int index;

    [Header("Idle Timer")]
    public float maxIdleTime;

    // Start is called before the first frame update
    void Start()
    {
        index = Random.Range(0, waypoints.Length);
        transform.position = waypoints[index];
        ChooseNextIndex();
    }

    // called in chicken_idle.cs animation behavior script
    public void ChooseNextIndex()
    {
        int nextIndex = Random.Range(0, waypoints.Length);
        if (nextIndex != index)
        {   
            // flip sprite so that object is facing the right way
            if (waypoints[nextIndex].x > waypoints[index].x)
                GetComponent<SpriteRenderer>().flipX = true;
            else
                GetComponent<SpriteRenderer>().flipX = false;

            // set new index
            index = nextIndex;
        }
        else
        {
            // recursive to avoid going to same index
            ChooseNextIndex();  
        }
    }
}
