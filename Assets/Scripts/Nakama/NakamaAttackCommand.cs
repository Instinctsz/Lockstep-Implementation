using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class NakamaAttackCommand : MonoBehaviour
{
    void Start()
    {
        InputHandler.AttackCommand += SendAttackCommand;
        NakamaConnection.ClientSocket.ReceivedMatchState += HandleAttackCommand;
    }

    async void SendAttackCommand(Unit unitToAttack)
    {
        AttackState attackState = new AttackState(unitToAttack);
        string matchId = NakamaMatchHandler.Match.Id;

        Debug.Log("Sending Attack Packet: " + attackState.Serialize());

        await NakamaConnection.ClientSocket.SendMatchStateAsync(matchId, Opcodes.Attack, attackState.Serialize(), NakamaMatchHandler.UsersInMatch);
    }

    void HandleAttackCommand(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Attack)
            return;

        MainThread.Enqueue(() =>
        {
            Unit unitToAttack = AttackState.Deserialize(newState.State);
            GameObject attacker = NakamaMatchHandler.Players[newState.UserPresence.SessionId];

            Debug.Log("Received Attack Packet, unit to attack: " + unitToAttack.guid);
            Debug.Log("====================================");

            Unit attackerUnit = attacker.GetComponentInChildren<Unit>();
            attackerUnit.SetState(new AttackState(unitToAttack));
            attackerUnit.ExecuteCurrentState();
        });
    }
}

