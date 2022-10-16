using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class NakamaMatchHandler : MonoBehaviour
{
    public static IMatch Match;
    public static event Action<IMatchState> MatchStart = delegate { };
    public static event Action<IMatch> MatchJoined = delegate { };
    public static event Action<IUserPresence> MatchPlayerJoined = delegate { };
    public static event Action<IUserPresence> MatchPlayerLeft = delegate { };

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
        string matchId = await RpcMatchCall("CreateMatch");
        Match = await NakamaConnection.ClientSocket.JoinMatchAsync(matchId);

        foreach (var user in Match.Presences)
        {
            UsersInMatch.Add(user);
        }

        UsersInMatch.Add(Match.Self);
        MatchJoined.Invoke(Match);
    }

    public async void JoinMatch()
    {
        string matchId = await RpcMatchCall("GetMatchByName");
        Match = await NakamaConnection.ClientSocket.JoinMatchAsync(matchId);

        foreach (var user in Match.Presences)
        {
            UsersInMatch.Add(user);
        }

        UsersInMatch.Add(Match.Self);
        MatchJoined.Invoke(Match);
    }
    public async void SendStartMatchPacket()
    {
        await NakamaConnection.ClientSocket.SendMatchStateAsync(Match.Id, Opcodes.Start_Match, "{}", UsersInMatch);
    }

    // Updating list of players
    public void ReceivedMatchPresence(IMatchPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            Debug.Log("Player left");
            UsersInMatch.Remove(presence);
            MatchPlayerLeft.Invoke(presence);
        }

        foreach (var presence in presenceEvent.Joins)
        {
            if (!presence.Equals(Match.Self))
            {
                Debug.Log("Player joined");
                UsersInMatch.Add(presence);
                MatchPlayerJoined.Invoke(presence);
            }
        }
    }

    public void ReceivedMatchState(IMatchState newState)
    {
        if (newState.OpCode == Opcodes.Start_Match)
            MatchStart.Invoke(newState);
    }

    public static IUserPresence FindUserBySession(string sessionId)
    {
        return UsersInMatch.Find(user => user.SessionId == sessionId);
    }

    async Task<String> RpcMatchCall(string rpcName)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("MatchName", defaultMatchName);

        IApiRpc response = await NakamaConnection.Client.RpcAsync(NakamaConnection.Session, rpcName, JsonWriter.ToJson(payload));
        Dictionary<string, string> responseParsed = JsonParser.FromJson<Dictionary<string, string>>(response.Payload);

        return responseParsed["matchId"];
    }

    public void OnDestroy()
    {
      
    }
}
