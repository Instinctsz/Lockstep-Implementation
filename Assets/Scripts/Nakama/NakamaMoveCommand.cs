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

        await NakamaConnection.ClientSocket.SendMatchStateAsync(matchId, Opcodes.Position, positionState.Serialize(), NakamaMatchHandler.UsersInMatch);
    }

    void HandleMoveCommand(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Position)
            return;

        MainThread.Enqueue(() =>
        {
            Vector3 positionToMoveTo = PositionState.Deserialize(newState.State);
            GameObject unitGo = NakamaMatchHandler.Players[newState.UserPresence.SessionId];

            Unit unit = unitGo.GetComponent<Unit>();
            unit.MoveTo(positionToMoveTo);
        });
    }
}
