using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Text;

[Serializable]
public class PositionState : State
{
    public float X;
    public float Y;
    public float Z;

    public PositionState(Vector3 position)
    {
        X = position.x;
        Y = position.y;
        Z = position.z;
    }

    public override string Serialize()
    {
        return JsonWriter.ToJson(this);
    }

    public static Vector3 Deserialize(byte[] state)
    {
        string stateJson = Encoding.UTF8.GetString(state);
        PositionState positionState = JsonParser.FromJson<PositionState>(stateJson);

        return new Vector3(positionState.X, positionState.Y, positionState.Z);
    }
}
