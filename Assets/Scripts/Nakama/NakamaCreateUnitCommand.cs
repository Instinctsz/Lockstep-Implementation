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

        Debug.Log("Sending Create Unit Packet: " + createUnitState.Serialize());

        await NakamaConnection.ClientSocket.SendMatchStateAsync(matchId, Opcodes.Create_Unit, createUnitState.Serialize(), NakamaMatchHandler.UsersInMatch);
    }

    public void HandleCreateUnitCommand(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Create_Unit)
            return;

        if (NakamaHelper.IsMainThread())
            CreateUnitCommand(newState);
        else
            MainThread.Enqueue(() => CreateUnitCommand(newState));
    }

    void CreateUnitCommand(IMatchState newState)
    {
        CreateUnitState createUnitState = CreateUnitState.Deserialize(newState.State);

        GameObject go = Instantiate(unitPrefab);

        Unit unit = go.GetComponentInChildren<Unit>();
        unit.transform.position = createUnitState.GetPosition();
        unit.Team = (Team)createUnitState.Team;
        unit.guid = createUnitState.GUID;
        go.GetComponentInChildren<UnitUI>().SetName(NakamaMatchHandler.FindUserBySession(newState.UserPresence.SessionId).Username);

        Debug.Log("Received Create Unit Packet: " + createUnitState.Serialize());
        Debug.Log("====================================");

        NakamaMatchHandler.Players.Add(newState.UserPresence.SessionId, go);
        UnitCreated.Invoke(unit);
    }
}
