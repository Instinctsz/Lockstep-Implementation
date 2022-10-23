using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Text;
using Nakama.TinyJson;
using System;

public class NakamaTurnHandler : MonoBehaviour
{
    public static event Action<int> TurnTicked = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        NakamaConnection.ClientSocket.ReceivedMatchState += HandleTurnChange;
    }

    public void HandleTurnChange(IMatchState matchState)
    {
        if (matchState.OpCode != Opcodes.Turn_Timer_Tick)
            return;

        string stateJson = Encoding.UTF8.GetString(matchState.State);
        int currentTick = Int32.Parse(stateJson);
        NakamaServerManager.CurrentTick = currentTick;
        TurnTicked.Invoke(currentTick);
    }   
}
