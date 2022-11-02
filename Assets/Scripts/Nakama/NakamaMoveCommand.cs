using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Text;
using Nakama.TinyJson;

public class NakamaMoveCommand : MonoBehaviour
{
    void Start()
    {
        InputHandler.MovementCommand += SendMoveCommand;
        NakamaConnection.ClientSocket.ReceivedMatchState += HandleMoveCommand;
    }

    async void SendMoveCommand(Vector3 position)
    {
        PositionState positionState = new PositionState(position);
        string matchId = NakamaMatchHandler.Match.Id;

        Debug.Log("Sending Move Packet: " + positionState.Serialize());

        await NakamaConnection.ClientSocket.SendMatchStateAsync(matchId, Opcodes.Position, positionState.Serialize(), NakamaMatchHandler.UsersInMatch);
    }

    public void HandleMoveCommand(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Position)
            return;

        MainThread.Enqueue(() =>
        {
            Vector3 positionToMoveTo = PositionState.Deserialize(newState.State);
            Debug.Log("reading from player: " + newState.UserPresence.SessionId);
            GameObject unitGo = NakamaMatchHandler.Players[newState.UserPresence.SessionId];

            if (unitGo == null) return;

            Debug.Log("Received Move Packet: " + positionToMoveTo + ", for player: " + newState.UserPresence.Username);
            Debug.Log("====================================");

            Unit unit = unitGo.GetComponentInChildren<Unit>();
            unit.SetState(new PositionState(positionToMoveTo));
            unit.ExecuteCurrentState();
        });
    }
}
