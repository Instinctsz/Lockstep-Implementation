using Nakama;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;





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

        socket = Socket.From(client);

        socket.ReceivedMatchPresence += ReceivedMatchPresence;
        socket.ReceivedMatchState += ReceivedMatchState;

        var session = await client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);

        bool appearOnline = true;
        int connectionTimeout = 30;
        await socket.ConnectAsync(session, appearOnline, connectionTimeout);
    }

    public async void StartMatchClicked()
    {
        await socket.SendMatchStateAsync(match.Id, Opcodes.Start_Match, "", usersInMatch);

        foreach (var user in match.Presences)
        {
            Debug.Log("Connected user: " + user.SessionId);
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
                Debug.Log("Received start match packet");
                //StartMatch();
                break;
            case Opcodes.Position:
                HandlePositionUpdate(newState);
                break;
            default:
                break;

        }
    }

    void HandlePositionUpdate(IMatchState newState) {

        GameObject unit = players[newState.UserPresence.SessionId];

        var stateJson = Encoding.UTF8.GetString(newState.State);
        var positionState = JsonParser.FromJson<PositionState>(stateJson);
        Vector3 newPosition = new Vector3(positionState.X, positionState.Y, positionState.Z);

        mainThreadCalls.Enqueue(() =>
        {
            unit.transform.position = newPosition;
        });
        
    }

    public async void SendPositionUpdate(Vector3 position) {
        PositionState state = new PositionState(position);

        await socket.SendMatchStateAsync(match.Id, Opcodes.Position, state.Serialize());
    }

    public void Update()
    {
        while (mainThreadCalls.Count > 0)
        {
            Action action = mainThreadCalls.Dequeue();
            action();
        }
    }

    private void OnDestroy()
    {
        
    }
}

