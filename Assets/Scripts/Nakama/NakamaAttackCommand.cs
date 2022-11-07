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

    public void HandleAttackCommand(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Attack)
            return;

        if (NakamaHelper.IsMainThread())
            AttackCommand(newState);
        else
            MainThread.Enqueue(() => AttackCommand(newState));
    }

    public void AttackCommand(IMatchState newState)
    {
        Unit unitToAttack = AttackState.Deserialize(newState.State);
        GameObject attacker = NakamaMatchHandler.Players[newState.UserPresence.SessionId];

        if (attacker == null) return;

        Debug.Log("Received Attack Packet, unit to attack: " + unitToAttack.guid);
        Debug.Log("====================================");

        Unit attackerUnit = attacker.GetComponentInChildren<Unit>();
        attackerUnit.SetState(new AttackState(unitToAttack));
        attackerUnit.ExecuteCurrentState();
    }
}

