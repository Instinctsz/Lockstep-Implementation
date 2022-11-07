using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class NakamaHelper : MonoBehaviour
{
    [SerializeField] NakamaCreateUnitCommand UnitCommand;
    [SerializeField] NakamaMoveCommand MoveCommand;
    [SerializeField] NakamaAttackCommand AttackCommand;

    static NakamaCreateUnitCommand unitCommand;
    static NakamaMoveCommand moveCommand;
    static NakamaAttackCommand attackCommand;

    public static int mainThreadId;

    void Start()
    {
        mainThreadId = GetCurrentThread();
        Debug.Log("Main thread is: " + mainThreadId);
        unitCommand = UnitCommand;
        moveCommand = MoveCommand;
        attackCommand = AttackCommand;
    }
    public static int GetCurrentThread()
    {
        return System.Threading.Thread.CurrentThread.ManagedThreadId;
    }

    public static bool IsMainThread()
    {
        return GetCurrentThread() == mainThreadId;
    }

    public static void SendCommandFromMatchstate(IMatchState state)
    {
        if (state.OpCode == Opcodes.Create_Unit)
        {
            NakamaMatchHandler.UsersInMatch.Add(state.UserPresence);
            unitCommand.HandleCreateUnitCommand(state);
        }
        if (state.OpCode == Opcodes.Position)
        {
            moveCommand.HandleMoveCommand(state);
        }
        if (state.OpCode == Opcodes.Attack)
        {
            attackCommand.HandleAttackCommand(state);
        }
    }
}

public class LockstepMatchState : IMatchState
{
    public string MatchId { get; set; }

    public long OpCode { get; set; }

    public byte[] State { get; set; }

    public IUserPresence UserPresence { get; set; }

    public LockstepMatchState(string matchId, long opcode, byte[] state, IUserPresence userPresence)
    {
        MatchId = matchId;
        OpCode = opcode;
        State = state;
        UserPresence = userPresence;
    }
}

public class LockstepUserPresence : IUserPresence
{
    public bool Persistence { get; set; }

    public string SessionId { get; set; }

    public string Status { get; set; }

    public string Username { get; set; }

    public string UserId { get; set; }

    public LockstepUserPresence(bool persistance, string sessionId, string username, string userId, string status = "")
    {
        Persistence = persistance;
        SessionId = sessionId;
        Username = username;
        UserId = userId;
        Status = status;
    }
}