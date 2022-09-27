using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Team PlayerTeam;
    public static PlayerManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        PlayerTeam = Team.BLUE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
