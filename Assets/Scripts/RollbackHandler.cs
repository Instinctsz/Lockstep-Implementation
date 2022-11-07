using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Text;
using System;

public class RollbackHandler : MonoBehaviour
{
    public float lerpDuration = 0.2f;

    private float turnDurationSeconds;

    void Start()
    {
        NakamaConnection.ClientSocket.ReceivedMatchState += HandleRollback;
        turnDurationSeconds = 1f / 10f;
    }

    void HandleRollback(IMatchState newState)
    {
        if (newState.OpCode != Opcodes.Rollback_Request)
            return;

        MainThread.Enqueue(() => {
            string stateJson = Encoding.UTF8.GetString(newState.State);
            RollbackState rollbackState = RollbackState.Deserialize(newState.State);
            int tickToRollbackTo = rollbackState.TickToQueueTo;

            Debug.Log("Rolling back to: " + tickToRollbackTo);
            Debug.Log("Current tick is: " + NakamaServerManager.CurrentTick);

            List<RollbackSave> rollbackSaves = NakamaServerManager.GetRollbackSaves(tickToRollbackTo);

            foreach (RollbackSave save in rollbackSaves)
            {
                Unit unit = MatchManager.Units[save.Guid];
                unit.transform.position = save.Position;
                unit.transform.rotation = save.Rotation;
                unit.ChangeHp(save.Hp);
                unit.SetState(save.CurrentState);
                //StartCoroutine(LerpPosition(unit, unit.transform.position, save.Position));
                //StartCoroutine(LerpRotation(unit, unit.transform.rotation, save.Rotation));
                //unit.ExecuteCurrentState()
            }

            //Execute the action that was late
            LockstepMatchState matchState = new LockstepMatchState(NakamaMatchHandler.Match.Id, rollbackState.OpCode, newState.State, newState.UserPresence);
            NakamaHelper.SendCommandFromMatchstate(matchState);
            Debug.Log("After send command");

            // Catch up
            int counter = tickToRollbackTo;
            while (counter <= NakamaServerManager.CurrentTick)
            {
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

            Debug.Log("Checking unit state in simulate turn : " + turn);
            Debug.Log("Unit position is: " + unit.transform.position);

            if (unit.CurrentState == null) return;

            Debug.Log("Simulating unit movement");
            unit.movementHandler.ProcessMovementInput(turnDurationSeconds);

            if (turn < NakamaServerManager.CurrentTick)
                NakamaServerManager.SetStateOnRollbackSave(turn + 1, unit);
        }
    }

    IEnumerator LerpPosition(Unit unit, Vector3 startPosition, Vector3 positionToLerpTo)
    {
        Debug.Log("Start lerp position");
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            unit.transform.position = Vector3.Lerp(startPosition, positionToLerpTo, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = positionToLerpTo;
        Debug.Log("End lerp position");
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


