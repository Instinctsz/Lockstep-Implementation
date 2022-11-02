using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NakamaServerManager
{
    public static int CurrentTick = 0;
    public static int TickDelay = 3;

    private static Dictionary<int, List<RollbackSave>> rollbackSaves = new Dictionary<int, List<RollbackSave>>();

    public static void AddRollbackSave(int tick, RollbackSave rollbackSave)
    {
        if (!rollbackSaves.ContainsKey(tick))
        {
            rollbackSaves[tick] = new List<RollbackSave>();
        }

        rollbackSaves[tick].Add(rollbackSave);
    }

    public static List<RollbackSave> GetRollbackSaves(int tick)
    {
        return rollbackSaves[tick];
    }

    public static void SetStateOnRollbackSave(int tick, Unit unit, State state)
    {
        Debug.Log("Setting " + state + "for tick: " + tick);
        List<RollbackSave> saves = GetRollbackSaves(tick);

        RollbackSave save = saves.First(rollbacksave => rollbacksave.Guid == unit.guid);
        saves.Remove(save);
        save.CurrentState = state;

        saves.Add(save);
        rollbackSaves[tick] = saves;
    }
}

public class RollbackSave
{
    public string Guid;
    public Vector3 Position;
    public Quaternion Rotation;
    public State CurrentState;
    public int Hp;

    public RollbackSave(string _guid, Vector3 _position, Quaternion _rotation, int _hp, State _state = null)
    {
        Guid = _guid;
        Position = _position;
        Rotation = _rotation;
        CurrentState = _state;
        Hp = _hp;
    }
}