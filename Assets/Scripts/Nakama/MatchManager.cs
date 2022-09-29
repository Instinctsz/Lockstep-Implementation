using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static Dictionary<string, Unit> Units = new Dictionary<string, Unit>();

    void Start()
    {
        NakamaCreateUnitCommand.UnitCreated += OnUnitCreated;    
    }

    void OnUnitCreated(Unit unit)
    {
        Debug.Log("Adding unit with guid: " + unit.guid);
        Units.Add(unit.guid, unit);
    }
}
