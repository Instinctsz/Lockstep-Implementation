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

    private Movement movementHandler;

    public event Action<int> HealthChanged = delegate { };
    public event Action AttackStart = delegate { };

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
        movementHandler.OnArrivedTarget -= DealDamage;

        if (Vector3.Distance(unit.transform.position, transform.position) > AttackRange)
        {
            movementHandler.MoveTo(unit, AttackRange);
            movementHandler.OnArrivedTarget += DealDamage;
        }
        else
        {
            DealDamage(unit);
        }
    }

    void DealDamage(Unit unit)
    {
        AttackStart.Invoke();
        unit.TakeDamage(AttackPower);
    } 

    public void MoveTo(Vector3 pos)
    {
        pos.y += 0.5f;
        movementHandler.MoveTo(pos);
    }
 
    void TakeDamage(int amount)
    {
        Hp -= amount;

        HealthChanged.Invoke(Hp);

        if (Hp <= 0)
            Die();
    }

    void Die()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnDestroy()
    {

    }
}
