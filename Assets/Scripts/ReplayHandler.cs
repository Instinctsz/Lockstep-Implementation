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

        if (replayData.Count == 0) return;

        int packetTurn = int.Parse(replayData[0].key) - offsetReplay;
        if (packetTurn == currentTurn)
        {
            StorageItem item = replayData[0];
            foreach (Packet packet in item.value.packets)
            {
                LockstepUserPresence userPresence = new LockstepUserPresence(true, packet.sender.sessionId, packet.sender.username, packet.sender.userId);
                LockstepMatchState matchState = new LockstepMatchState(match.Id, packet.opCode, Encoding.ASCII.GetBytes(packet.data), userPresence);

                NakamaHelper.SendCommandFromMatchstate(matchState);
            }
            replayData.RemoveAt(0);
        }

    }
}



