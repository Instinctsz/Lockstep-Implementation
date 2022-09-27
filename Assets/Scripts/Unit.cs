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

    private Movement movementHandler;

    private float nextActionTime = 0.0f;
    private float period = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        if (Team == PlayerManager.PlayerTeam)
        {
            InputHandler.Instance.MovementCommand += OnMovementCommand;
            InputHandler.Instance.AttackCommand += OnAttackCommand;
            movementHandler = GetComponent<Movement>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Team == PlayerManager.PlayerTeam)
        {
             NakamaTest.Instance.SendPositionUpdate(transform.position);    
        }
    }

    void OnMovementCommand(Vector3 pos)
    {
        MoveTo(pos);
    }

    void OnAttackCommand(Unit unit)
    {
        movementHandler.OnArrivedTarget -= Attack;

        if (Vector3.Distance(unit.transform.position, transform.position) > AttackRange)
        {
            movementHandler.MoveTo(unit, AttackRange);
            movementHandler.OnArrivedTarget += Attack;
        }
        else
        {
            Attack(unit);
        }
    }

    void Attack(Unit unit)
    {
        unit.TakeDamage(AttackPower);
    } 

    void MoveTo(Vector3 pos)
    {
        pos.y += 2;
        movementHandler.MoveTo(pos);
    }
 
    void TakeDamage(int amount)
    {
        Hp -= amount;

        if (Hp <= 0)
            Die();
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        InputHandler.Instance.MovementCommand -= OnMovementCommand;
        InputHandler.Instance.AttackCommand -= Attack;
    }
}
