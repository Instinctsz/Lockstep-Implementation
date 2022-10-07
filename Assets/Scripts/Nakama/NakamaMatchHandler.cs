using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public async void CreateMatch()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("MatchName", defaultMatchName);

        IApiRpc response = await NakamaConnection.Client.RpcAsync(NakamaConnection.Session, "CreateMatch", JsonWriter.ToJson(payload));
        Dictionary<string, string> responseParsed = JsonParser.FromJson<Dictionary<string, string>>(response.Payload);
        string matchId = responseParsed["matchId"];

        Debug.Log("Created match with id: " + matchId);

        Match = await NakamaConnection.ClientSocket.JoinMatchAsync(matchId);

        foreach (var user in Match.Presences)
        {
            UsersInMatch.Add(user);
        }

        UsersInMatch.Add(Match.Self);

    }

    public async void JoinMatch()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("MatchName", defaultMatchName);

        IApiRpc response = await NakamaConnection.Client.RpcAsync(NakamaConnection.Session, "GetMatchByName", JsonWriter.ToJson(payload));
        Dictionary<string, string> responseParsed = JsonParser.FromJson<Dictionary<string, string>>(response.Payload);
        string matchId = responseParsed["matchId"];

        Debug.Log("Joined match with id: " + matchId);

        Match = await NakamaConnection.ClientSocket.JoinMatchAsync(matchId);

        foreach (var user in Match.Presences)
        {
            UsersInMatch.Add(user);
        }

        UsersInMatch.Add(Match.Self);
    }
    public async void SendStartMatchPacket()
    {
        await NakamaConnection.ClientSocket.SendMatchStateAsync(Match.Id, Opcodes.Start_Match, "1", UsersInMatch);
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
        if (newState.OpCode == Opcodes.Start_Match)
            MatchStart.Invoke(newState);
    }

    public void OnDestroy()
    {
      
    }
}
