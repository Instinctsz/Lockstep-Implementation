using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Movement))]
public class Unit : MonoBehaviour
{
    public int Hp;
    public int AttackPower;
    public int AttackRange;
    public Team Team; 
   
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public string guid;

    public event Action<int> HealthChanged = delegate { };
    public event Action<int> TakenDamage = delegate { };
    public event Action AttackStart = delegate { };

    public State CurrentState;

    private Movement movementHandler;


    // Start is called before the first frame update
    void Start()
    {
        movementHandler = GetComponent<Movement>();
        MaxHealth = Hp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(Unit unit)
    {
        if (Vector3.Distance(unit.transform.position, transform.position) > AttackRange)
        {
            movementHandler.MoveTo(unit, AttackRange);
            movementHandler.OnArrivedTarget += DealDamage;
        }
        else
        {
            transform.LookAt(unit.transform);
            DealDamage(unit);
        }
    }

    public void ChangeHp(int newHp)
    {
        Hp = newHp;

        HealthChanged.Invoke(Hp);

        if (Hp <= 0)
            Die();
    }

    void DealDamage(Unit unit)
    {
        AttackStart.Invoke();
        unit.TakeDamage(AttackPower);

        SetState(null);
    } 

    public void MoveTo(Vector3 pos)
    {
        pos.y += 0.5f;
        movementHandler.MoveTo(pos);
        movementHandler.OnArrived += UnitArrivedAtDestination;
    }

    void UnitArrivedAtDestination()
    {
        movementHandler.OnArrived -= UnitArrivedAtDestination;
        SetState(null);
    }
 
    void TakeDamage(int amount)
    {
        Hp -= amount;

        TakenDamage.Invoke(Hp);

        if (Hp <= 0)
            Die();
    }

    void Die()
    {
        if (Team == PlayerManager.PlayerTeam)
        {
            CameraController.AllowMovement = false;
            InputHandler.StopCapturingInput();
        }

        Destroy(transform.parent.gameObject);
    }

    public void SetState(State newState)
    {
        CurrentState = newState;

        movementHandler.OnArrivedTarget -= DealDamage;
        movementHandler.OnArrived -= UnitArrivedAtDestination;
    }

    public void ExecuteCurrentState()
    {
        if (CurrentState == null)
            return;

        Type stateType = CurrentState.GetType();

        if (stateType == typeof(PositionState))
        {
            PositionState positionState = (PositionState)CurrentState;
            MoveTo(new Vector3(positionState.X, positionState.Y, positionState.Z));
        }
        if (stateType == typeof(AttackState))
        {
            AttackState attackState = (AttackState)CurrentState;
            Unit unitToAttack = MatchManager.Units[attackState.AttackedUnitGUID];
            Attack(unitToAttack);
        }
    }

    private void OnDestroy()
    {

    }
}
