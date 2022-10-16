using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Text;

[Serializable]
public class CreateUnitState : State
{
    public string GUID;
    public int Team;
    public float X;
    public float Y;
    public float Z;

    public CreateUnitState(string guid, Vector3 position, Team team)
    {
        GUID = guid;
        Team = (int)team;
        X = position.x;
        Y = position.y;
        Z = position.z;
    }

    public override string Serialize()
    {
        return JsonWriter.ToJson(this);
    }

    public static CreateUnitState Deserialize(byte[] state)
    {
        string stateJson = Encoding.UTF8.GetString(state);
        CreateUnitState createUnitState = JsonParser.FromJson<CreateUnitState>(stateJson);

        return createUnitState;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(X, Y, Z);
    }
}
