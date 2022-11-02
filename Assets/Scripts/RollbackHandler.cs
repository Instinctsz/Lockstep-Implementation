using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Text;
using System;

public class RollbackHandler : MonoBehaviour
{
    public float lerpDuration = 0.2f;

    private float turnDurationSeconds = 1 / 10;

    void Start()
    {
        NakamaConnection.ClientSocket.ReceivedMatchState += HandleRollback;
    }

    void HandleRollback(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Rollback_Request)
            return;

        MainThread.Enqueue(() => {
            string stateJson = Encoding.UTF8.GetString(newState.State);
            int tickToRollbackTo = Int32.Parse(stateJson);

            Debug.Log("Rolling back to: " + tickToRollbackTo);

            List<RollbackSave> rollbackSaves = NakamaServerManager.GetRollbackSaves(tickToRollbackTo);

            foreach (RollbackSave save in rollbackSaves)
            {
                Unit unit = MatchManager.Units[save.Guid];
                StartCoroutine(LerpPosition(unit, unit.transform.position, save.Position));
                StartCoroutine(LerpRotation(unit, unit.transform.rotation, save.Rotation));
                unit.ChangeHp(save.Hp);
                unit.SetState(save.CurrentState);
                //unit.ExecuteCurrentState()
            }
            Debug.Log("Finished processing tick: " + tickToRollbackTo);

            // Catch up
            int counter = tickToRollbackTo;
            while (counter <= NakamaServerManager.CurrentTick)
            {
                Debug.Log("Simulating tick: " + counter);
                SimulateTurn(counter);
                counter++;
            }
        });
    }

    void SimulateTurn(int turn)
    {
        List<RollbackSave> rollbackSaves = NakamaServerManager.GetRollbackSaves(turn);

        foreach (RollbackSave save in rollbackSaves)
        {
            Unit unit = MatchManager.Units[save.Guid];

            if (unit.CurrentState == null) return;

            Debug.Log("Simulating unit movement");
            unit.movementHandler.ProcessMovementInput(turnDurationSeconds);
            NakamaServerManager.SetStateOnRollbackSave(turn + 1, unit, unit.CurrentState);
        }
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
