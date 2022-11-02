using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Nakama;
using System;
using System.Threading.Tasks;
using Nakama.TinyJson;
using System.Text;

public class ReplayHandler : MonoBehaviour
{
    [SerializeField] GameObject[] toDisable;

    [SerializeField] NakamaCreateUnitCommand unitCommand;
    [SerializeField] NakamaMoveCommand moveCommand;
    [SerializeField] NakamaAttackCommand attackCommand;

    List<StorageItem> replayData;
    int currentTurn = 0;
    int offsetReplay = 0;
    IMatch match;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void StartReplay(StorageItem[] _replayData)
    {
        Array.ForEach(toDisable, go => go.SetActive(false));

        string matchId = await NakamaMatchHandler.RpcMatchCall("CreateMatch", "amogus");
        match = await NakamaConnection.ClientSocket.JoinMatchAsync(matchId);

        replayData = _replayData.ToList();
        offsetReplay = int.Parse(replayData[0].key) - 1;
        InvokeRepeating("TurnPassed", 0, 0.1f);
    }

    void TurnPassed()
    {
        currentTurn++;
        Debug.Log("Current turn: " + currentTurn);

        if (replayData.Count == 0) return;

        int packetTurn = int.Parse(replayData[0].key) - offsetReplay;
        if (packetTurn == currentTurn)
        {
            StorageItem item = replayData[0];
            foreach (Packet packet in item.value.packets)
            {
                Debug.Log("Sending packet with opcode: " + packet.opCode);
                ReplayUserPresence userPresence = new ReplayUserPresence(true, packet.sender.sessionId, packet.sender.username, packet.sender.userId);
                ReplayMatchState matchState = new ReplayMatchState(match.Id, packet.opCode, Encoding.ASCII.GetBytes(packet.data), userPresence);

                if (packet.opCode == Opcodes.Create_Unit)
                {
                    NakamaMatchHandler.UsersInMatch.Add(userPresence);
                    unitCommand.HandleCreateUnitCommand(matchState);
                }
                if (packet.opCode == Opcodes.Position)
                    moveCommand.HandleMoveCommand(matchState);
                if (packet.opCode == Opcodes.Attack)
                {
                    Debug.Log("Received attack command from : " + packet.sender.username);
                    attackCommand.HandleAttackCommand(matchState);
                }
            }
            replayData.RemoveAt(0);
        }

    }
}

class ReplayMatchState : IMatchState
{
    public string MatchId { get; set; }

    public long OpCode { get; set; }

    public byte[] State { get; set; }

    public IUserPresence UserPresence { get; set; }

    public ReplayMatchState(string matchId, long opcode, byte[] state, IUserPresence userPresence)
    {
        MatchId = matchId;
        OpCode = opcode;
        State = state;
        UserPresence = userPresence;
    }
}

class ReplayUserPresence : IUserPresence
{
    public bool Persistence { get; set; }

    public string SessionId { get; set; }

    public string Status { get; set; }

    public string Username { get; set; }

    public string UserId { get; set; }

    public ReplayUserPresence(bool persistance, string sessionId, string username, string userId, string status = "")
    {
        Persistence = persistance;
        SessionId = sessionId;
        Username = username;
        UserId = userId;
        Status = status;
    }
}

