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
       
        if (NakamaHelper.IsMainThread())
            MoveCommand(newState);
        else
            MainThread.Enqueue(() => MoveCommand(newState));
    }

    public void MoveCommand(IMatchState newState)
    {
        Vector3 positionToMoveTo = PositionState.Deserialize(newState.State);
        GameObject unitGo = NakamaMatchHandler.Players[newState.UserPresence.SessionId];

        if (unitGo == null) return;

        Debug.Log("Received Move Packet: " + positionToMoveTo + ", for player: " + newState.UserPresence.Username);
        Debug.Log("====================================");

        Unit unit = unitGo.GetComponentInChildren<Unit>();
        unit.SetState(new PositionState(positionToMoveTo));
        Debug.Log("Set the state to position state");
        unit.ExecuteCurrentState();
    }
}
