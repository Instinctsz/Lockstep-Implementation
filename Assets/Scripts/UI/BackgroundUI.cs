using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class BackgroundUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NakamaMatchHandler.MatchStart += OnMatchStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMatchStart(IMatchState state)
    {
        MainThread.Enqueue(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
