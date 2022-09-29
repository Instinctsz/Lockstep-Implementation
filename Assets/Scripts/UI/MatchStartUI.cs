using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;

public class MatchStartUI : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        NakamaMatchHandler.MatchStart += HandleStartMatch;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void HandleStartMatch(IMatchState match)
    {
        MainThread.Enqueue(() => { HideMainMenuUI(); });     
    }

    public void HideMainMenuUI()
    {
        MainMenuUI.SetActive(false);
    }
}
