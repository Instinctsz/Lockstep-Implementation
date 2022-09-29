using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject spawnpoints;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            Debug.Log(spawnpoints.transform.GetChild(i).name);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
