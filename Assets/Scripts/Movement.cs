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
        if (positionToMoveTo != Vector3.zero)
        {
            transform.LookAt(positionToMoveTo);
            HandleMovement(positionToMoveTo);
        }

        if (target != null)
        {
            transform.LookAt(target.transform.position);
            HandleMovement(target.transform.position);
        }
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


    void HandleMovement(Vector3 pos)
    {
        float step = MovementSpeed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, pos, step);

        if (Vector3.Distance(transform.position, pos) < stopDistance)
        {
            if (target == null)
                OnArrived.Invoke();
            else
                OnArrivedTarget.Invoke(target);
            Reset();
        }
    }

    void Reset()
    {
        positionToMoveTo = Vector3.zero;
        target = null;
        stopDistance = defaultStopDistance;
    }
}
