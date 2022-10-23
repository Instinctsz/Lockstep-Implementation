using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RollbackTest))]
public class RollbackTestEditor : Editor
{
    string opcode = "6";
    string data = "50";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        opcode = EditorGUILayout.TextField("Opcode: ", opcode);
        data = EditorGUILayout.TextField("Data: ", data);

        if (GUILayout.Button("Send Packet"))
        {
            RollbackTest rollbackTest = (RollbackTest)target;
            rollbackTest.SendPacket(long.Parse(opcode), data);
            Debug.Log("Sending rollback packet");
        }
    }
}
