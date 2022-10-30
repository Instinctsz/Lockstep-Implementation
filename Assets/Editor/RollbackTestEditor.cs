using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RollbackTest))]
public class RollbackTestEditor : Editor
{
    string opcode = "6";
    string tick = "50";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        tick = EditorGUILayout.TextField("Tick: ", tick);

        if (GUILayout.Button("Send Packet"))
        {
            RollbackTest rollbackTest = (RollbackTest)target;
            rollbackTest.SendPacket(tick);
            Debug.Log("Sending rollback packet");
        }
    }
}
