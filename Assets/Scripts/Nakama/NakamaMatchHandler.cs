using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NakamaMatchHandler : MonoBehaviour
{
    public static IMatch Match;
    public static event Action<IMatchState> MatchStart = delegate { };

    public static Dictionary<string, GameObject> Players = new Dictionary<string, GameObject>();
    public static List<IUserPresence> UsersInMatch = new List<IUserPresence>();

    [SerializeField] private string defaultMatchName = "Amogus";

    // Start is called before the first frame update
    void Start()
    {
        NakamaConnection.ClientSocket.ReceivedMatchPresence += ReceivedMatchPresence;
        NakamaConnection.ClientSocket.ReceivedMatchState += ReceivedMatchState;
    }

    public async void FindMatch()
    {
        Debug.Log("Finding Match...");
        Match = await NakamaConnection.ClientSocket.CreateMatchAsync(defaultMatchName);

        foreach (var user in Match.Presences)
        {
            UsersInMatch.Add(user);
        }

        UsersInMatch.Add(Match.Self);
    }

    public async void SendStartMatchPacket()
    {
        await NakamaConnection.ClientSocket.SendMatchStateAsync(Match.Id, Opcodes.Start_Match, "", UsersInMatch);
    }

    // Updating list of players
    public void ReceivedMatchPresence(IMatchPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            Debug.Log("Player left");
            UsersInMatch.Remove(presence);
        }

        foreach (var presence in presenceEvent.Joins)
        {
            if (!presence.Equals(Match.Self))
            {
                Debug.Log("Player joined");
                UsersInMatch.Add(presence);
            }
        }
    }

    public void ReceivedMatchState(IMatchState newState)
    {
        Debug.Log("Received match state with opcode: " + newState.OpCode);
        if (newState.OpCode == Opcodes.Start_Match)
            MatchStart.Invoke(newState);
    }
}
