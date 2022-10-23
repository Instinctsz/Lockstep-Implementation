using System.Collections;
using System.Collections.Generic;
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

    public async void SendPacket(long opCode, string data)
    {
        string matchId = NakamaMatchHandler.Match.Id;
        await NakamaConnection.ClientSocket.SendMatchStateAsync(matchId, opCode, data, NakamaMatchHandler.UsersInMatch);
    }
}
