using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    RED,
    BLUE,
    YELLOW
}

[RequireComponent(typeof(Movement))]
public class Unit : MonoBehaviour
{
    public int Hp;
    public int AttackPower;
    public int AttackRange;
    public Team Team;

    private Movement movementHandler;

    // Start is called before the first frame update
    void Start()
    {
        if (Team == PlayerManager.Instance.PlayerTeam)
        {
            InputHandler.Instance.MovementCommand += OnMovementCommand;
            InputHandler.Instance.AttackCommand += OnAttackCommand;
            movementHandler = GetComponent<Movement>();
        }
    }

    // Update is called once per frame
    void Update()
    {

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
