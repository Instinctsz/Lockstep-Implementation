using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;

[Serializable]
public class AttackState : State
{
    public string AttackedUnitGUID;

    public AttackState(Unit attackedUnit)
    {
        AttackedUnitGUID = attackedUnit.guid;
    }

    public override string Serialize()
    {
        return JsonWriter.ToJson(this);
    }

    public static Unit Deserialize(byte[] state)
    {
        string stateJson = Encoding.UTF8.GetString(state);
        AttackState attackState = JsonParser.FromJson<AttackState>(stateJson);

        return MatchManager.Units[attackState.AttackedUnitGUID];
    }
}
