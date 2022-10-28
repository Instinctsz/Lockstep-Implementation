using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimations : MonoBehaviour
{

    [SerializeField] ParticleSystem particleSystemBarrel1;
    [SerializeField] ParticleSystem particleSystemBarrel2;
    [SerializeField] ParticleSystem DamageTaken;

    Unit unit;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
        unit.AttackStart += AttackStart;
        unit.TakenDamage += TakenDamage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakenDamage(int currentHp)
    {
        DamageTaken.Play();
    }

    void AttackStart()
    {
        animator.SetTrigger("Attack");
    }

    public void Barrel1Shoot()
    {
        particleSystemBarrel1.Play();
    }

    public void Barrel2Shoot()
    {
        particleSystemBarrel2.Play();
    }
}
