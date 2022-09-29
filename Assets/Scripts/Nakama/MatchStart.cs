using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class MatchStart : MonoBehaviour
{
    [SerializeField] private GameObject spawnpoints;

    // Start is called before the first frame update
    void Start()
    {
        NakamaMatchHandler.MatchStart += HandleStartMatch;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleStartMatch(IMatchState newState)
    {
        StartMatch();
    }

    public void StartMatch()
    {
        for (int i = 0; i < NakamaMatchHandler.UsersInMatch.Count; i++)
        {
            IUserPresence presence = NakamaMatchHandler.UsersInMatch[i];
            Team team = (Team)i;

            if (presence.Equals(NakamaMatchHandler.Match.Self))
            {
                Debug.Log("You are team " + team);
                PlayerManager.PlayerTeam = team;
                int childPosition = i;

                MainThread.Enqueue(() =>
                {
                    Vector3 position = spawnpoints.transform.GetChild(childPosition).position;
                    NakamaCreateUnitCommand.SendCreateUnitCommand(position, team);
                });
            }   
        }
    }
}
