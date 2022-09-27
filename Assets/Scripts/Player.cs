using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public enum Team
{
    RED,
    BLUE,
    YELLOW

}
public class Player
{
    public IUserPresence presence;
    public Team team;

    public Player(IUserPresence _presence, Team _team)
    {
        presence = _presence;
        team = _team;
    }
}
