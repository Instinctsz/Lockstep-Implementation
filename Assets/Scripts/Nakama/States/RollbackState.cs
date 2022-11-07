using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RollbackState : State
{
    public int OpCode;

    public static RollbackState Deserialize(byte[] state)
    {
        string stateJson = Encoding.UTF8.GetString(state);
        RollbackState rollbackState = JsonParser.FromJson<RollbackState>(stateJson);

        return rollbackState;
    }
}
