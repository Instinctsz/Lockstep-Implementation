using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public event Action OnArrived = delegate { };
    public event Action<Unit> OnArrivedTarget = delegate { };
    public int MovementSpeed = 10;

    private Unit target;
    private Vector3 positionToMoveTo = Vector3.zero;
    private float stopDistance;
    private float defaultStopDistance = 0.001f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessMovementInput(Time.fixedDeltaTime);
    }

    public void MoveTo(Vector3 pos, float _stopDistance = 0.001f)
    {
        Reset();
        positionToMoveTo = pos;
        stopDistance = _stopDistance;
    }

    public void MoveTo(Unit unit, float _stopDistance = 0.001f)
    {
        Reset();
        target = unit;
        stopDistance = _stopDistance;
    }


    public void HandleMovement(Vector3 pos, float timePassed)
    {
        float step = MovementSpeed * timePassed;
        transform.position = Vector3.MoveTowards(transform.position, pos, step);
        Debug.Log("Moved unit to: " + transform.position);

        if (Vector3.Distance(transform.position, pos) < stopDistance)
        {
            if (target == null)
                OnArrived.Invoke();
            else
                OnArrivedTarget.Invoke(target);
            Reset();
        }
    }

    public void ProcessMovementInput(float fixedDeltaTime)
    {
        if (positionToMoveTo != Vector3.zero)
        {
            transform.LookAt(positionToMoveTo);
            HandleMovement(positionToMoveTo, fixedDeltaTime);
        }

        if (target != null)
        {
            transform.LookAt(target.transform.position);
            HandleMovement(target.transform.position, fixedDeltaTime);
        }
    }

    void Reset()
    {
        positionToMoveTo = Vector3.zero;
        target = null;
        stopDistance = defaultStopDistance;
    }
}
