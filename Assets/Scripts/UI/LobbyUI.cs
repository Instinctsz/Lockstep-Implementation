using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] GameObject nextState;
    // Start is called before the first frame update
    void Start()
    {
        NakamaMatchHandler.MatchJoined += OnMatchJoined;
    }

    void OnMatchJoined(IMatch match)
    {
        nextState.SetActive(true);
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
