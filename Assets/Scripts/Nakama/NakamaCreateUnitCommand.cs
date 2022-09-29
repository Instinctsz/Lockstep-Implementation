using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;

public class NakamaCreateUnitCommand : MonoBehaviour
{
    public static event Action<Unit> UnitCreated = delegate { };

    [SerializeField] private GameObject unitPrefab;

    void Start()
    {
        NakamaConnection.ClientSocket.ReceivedMatchState += HandleCreateUnitCommand;
    }

    public static async void SendCreateUnitCommand(Vector3 position, Team team)
    {
        Guid guid = Guid.NewGuid();
        CreateUnitState createUnitState = new CreateUnitState(guid.ToString(), position, team);
        string matchId = NakamaMatchHandler.Match.Id;

        await NakamaConnection.ClientSocket.SendMatchStateAsync(matchId, Opcodes.Create_Unit, createUnitState.Serialize(), NakamaMatchHandler.UsersInMatch);
    }

    void HandleCreateUnitCommand(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Create_Unit)
            return;

        MainThread.Enqueue(() =>
        {
            CreateUnitState createUnitState = CreateUnitState.Deserialize(newState.State);

            GameObject go = Instantiate(unitPrefab);
            go.transform.position = createUnitState.GetPosition();

            Unit unit = go.GetComponent<Unit>();
            unit.Team = (Team)createUnitState.Team;
            unit.guid = createUnitState.GUID;

            NakamaMatchHandler.Players.Add(newState.UserPresence.SessionId, go);
            UnitCreated.Invoke(unit);
        });
    }
}
