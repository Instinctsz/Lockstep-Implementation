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
        Units.Add(unit.guid, unit);
    }
}
