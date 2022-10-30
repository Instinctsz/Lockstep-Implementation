using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RollbackSubscriber : MonoBehaviour
{
    Unit unit;
    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        NakamaTurnHandler.TurnTicked += OnTurnTick;
    }

    void OnTurnTick(int newTick)
    {
        MainThread.Enqueue(() =>
        {
            RollbackSave rollbackSave = new RollbackSave(unit.guid, transform.position, transform.rotation, unit.Hp, unit.CurrentState);
            NakamaServerManager.AddRollbackSave(newTick, rollbackSave);
        });
    }

    private void OnDestroy()
    {
        NakamaTurnHandler.TurnTicked -= OnTurnTick;
    }
}
