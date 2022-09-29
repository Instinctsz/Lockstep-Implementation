using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThread : MonoBehaviour
{
    private static Queue<Action> threadCalls = new Queue<Action>();

    // Update is called once per frame
    void Update()
    {
        while (threadCalls.Count > 0)
        {
            Action action = threadCalls.Dequeue();
            action();
        }
    }

    public static void Enqueue(Action action)
    {
        threadCalls.Enqueue(action);
    }
}
