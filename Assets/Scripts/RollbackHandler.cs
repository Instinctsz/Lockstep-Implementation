using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Text;
using System;

public class RollbackHandler : MonoBehaviour
{
    public float lerpDuration = 0.2f;

    void Start()
    {
        NakamaConnection.ClientSocket.ReceivedMatchState += HandleRollback;
    }

    void HandleRollback(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Rollback_Request)
            return;

        string stateJson = Encoding.UTF8.GetString(newState.State);
        int tickToRollbackTo = Int32.Parse(stateJson);

        List<RollbackSave> rollbackSaves = NakamaServerManager.GetRollbackSaves(tickToRollbackTo);

        MainThread.Enqueue(() => {
            foreach (RollbackSave save in rollbackSaves)
            {
                Unit unit = MatchManager.Units[save.Guid];
                StartCoroutine(LerpPosition(unit, unit.transform.position, save.Position));
                StartCoroutine(LerpRotation(unit, unit.transform.rotation, save.Rotation));
                //MatchManager.Units[save.Guid].Action = save.CurrentAction;
            }
        });
    }

    IEnumerator LerpPosition(Unit unit, Vector3 startPosition, Vector3 positionToLerpTo)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            unit.transform.position = Vector3.Lerp(startPosition, positionToLerpTo, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = positionToLerpTo;
    }

    IEnumerator LerpRotation(Unit unit, Quaternion startRotation, Quaternion rotationToLerpTo)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            unit.transform.rotation = Quaternion.Lerp(startRotation, rotationToLerpTo, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = rotationToLerpTo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
