using Nakama;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;

[Serializable]
public class PositionState
{
    public float X;
    public float Y;
    public float Z;
}


public class NakamaTest : MonoBehaviour
{
    public GameObject prefab;
    public GameObject spawnpoints;
    public GameObject HideUIOnStartMatch;

    public static NakamaTest Instance;
    private ISocket socket;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    private IMatch match;

    private List<IUserPresence> usersInMatch = new List<IUserPresence>();

    private Queue<Action> mainThreadCalls = new Queue<Action>();

    async void Start()
    {
        Instance = this;
        var client = new Client("http", "127.0.0.1", 7350, "defaultkey");
        client.Timeout = 10;

        //var logger = new UnityLogger();
        //client.Logger = logger;

        socket = Socket.From(client);

        socket.ReceivedMatchPresence += ReceivedMatchPresence;
        socket.ReceivedMatchState += ReceivedMatchState;


        var session = await client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);


        bool appearOnline = true;
        int connectionTimeout = 30;
        await socket.ConnectAsync(session, appearOnline, connectionTimeout);
    }

    public async void FindMatch()
    {
        var matchName = "Amogus";

        // When joining by match name, you use the CreateMatchAsync function instead of the JoinMatchAsync function
        match = await socket.CreateMatchAsync(matchName);

        foreach (var user in match.Presences)
        {
            Debug.Log("Connected user: " + user.SessionId);
            usersInMatch.Add(user);
        }

        usersInMatch.Add(match.Self);
    }

    public async void StartMatchClicked()
    {
        StartMatch();
        await socket.SendMatchStateAsync(match.Id, Opcodes.Start_Match, "", usersInMatch);

        foreach (var user in match.Presences)
        {
            Debug.Log("Connected user: " + user.SessionId);
        }
    }

    public void StartMatch()
    {
        int i = 0;
        HideUIOnStartMatch.SetActive(false);

        foreach (var presence in usersInMatch)
        {
            Team team = (Team)i;
            Player player = new Player(presence, team);

            DebugText.Instance.Add(i.ToString());
            DebugText.Instance.Add(presence.SessionId);
            if (presence.Equals(match.Self))
            {
                Debug.Log("You are team " + team);
                PlayerManager.PlayerTeam = team;
                DebugText.Instance.Add("You are team " + team);
                DebugText.Instance.Add(presence.SessionId);
            }

            var go = Instantiate(prefab);
            go.name = "Unit " + i;
            go.transform.position = spawnpoints.transform.GetChild(i).transform.position;
            DebugText.Instance.Add("Set the position");

            Unit unit = go.GetComponent<Unit>();
            unit.Team = team;

            players.Add(presence.SessionId, go);
            i++;
        }
    }

    public void ReceivedMatchPresence(IMatchPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            Debug.Log("Player left");
            usersInMatch.Remove(presence);
        }

        foreach (var presence in presenceEvent.Joins)
        {
            if (!presence.Equals(match.Self)) {
                Debug.Log("Player joined");
                usersInMatch.Add(presence);
            }
        }

        Debug.LogFormat("Connected opponents: [{0}]", string.Join(",\n  ", usersInMatch));
    }

    public void ReceivedMatchState(IMatchState newState)
    {
        var enc = System.Text.Encoding.UTF8;

        var content = enc.GetString(newState.State);

        switch (newState.OpCode)
        {
            case Opcodes.Start_Match:
                StartMatch();
                break;
            case Opcodes.Position:
                HandlePositionUpdate(newState);
                break;
            default:
                break;

        }
    }

    void HandlePositionUpdate(IMatchState newState) {

        //Player player = FindPlayerByPresence(newState.UserPresence);
        GameObject unit = players[newState.UserPresence.SessionId];

        if (unit == null)
        {
            Debug.LogError("NO UNIT FOUND");
        }

        var stateJson = Encoding.UTF8.GetString(newState.State);
        var positionState = JsonParser.FromJson<PositionState>(stateJson);
        Vector3 newPosition = new Vector3(positionState.X, positionState.Y, positionState.Z);

        Debug.Log("Received: " + newPosition.ToString());

        try
        {
            //Debug.Log(unit.name);
        }
        catch(Exception e)
        {
            Debug.Log("test");
            Debug.LogError(e);
        }

        Debug.Log("lol");

        mainThreadCalls.Enqueue(() =>
        {
            unit.transform.position = newPosition;
        });
        
    }

    public async void SendPositionUpdate(Vector3 position) {
        var state = new PositionState
        {
            X = position.x,
            Y = position.y,
            Z = position.z
        };

        await socket.SendMatchStateAsync(match.Id, Opcodes.Position, JsonWriter.ToJson(state));
    }

    public void Update()
    {
        while (mainThreadCalls.Count > 0)
        {
            Action action = mainThreadCalls.Dequeue();
            action();
        }
    }

    //Player FindPlayerByPresence(IUserPresence presence) {
    //    foreach(String player in players.Keys) 
    //    {
    //        if (player.presence.Equals(presence))
    //            return player;
    //    }

    //    return null;
    //}

    private void OnDestroy()
    {
        
    }
}

