using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RollbackTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void SendPacket(string tick)
    {
        PositionState positionState = new PositionState(Vector3.zero);
        positionState.TickToQueueTo = int.Parse(tick);
        string matchId = NakamaMatchHandler.Match.Id;
        await NakamaConnection.ClientSocket.SendMatchStateAsync(matchId, Opcodes.Position, positionState.Serialize(), NakamaMatchHandler.UsersInMatch);
    }
}

public class RollbackTestState
{
    public string TickToQueueTo;
    public float X = 0;
    public float Y = 0;
    public float Z = 0;

    public RollbackTestState(string tick)
    {
        TickToQueueTo = tick;
    }

    public string Serialize()
    {
        return JsonWriter.ToJson(this);
    }
}
