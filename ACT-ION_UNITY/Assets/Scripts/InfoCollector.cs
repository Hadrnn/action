using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InfoCollector : MonoBehaviour
{
    public List<GameObject> shells = new();
    public List<Team> teams = new();
    public List<GameObject> mapObjects = new();
    public string botMovement;
    public string gameResult = "Playing";

    public Vector3 team1Spawn { get; set; }
    public Vector3 team2Spawn { get; set; }

    private bool teamsSet = false;
    private static int NewTeamNumber = 0;
    private int NewOwnerID = 0;

    public class Team
    {
        public class Tank
        {
            public Tank(GameObject _tank)
            {
                tank = _tank;
                kills = 0;
                deaths = 0;
            }

            public GameObject tank;
            public int kills;
            public int deaths;
            public Team team;
        }
        public Team(int _teamNumber)
        {
            teamNumber = _teamNumber;
            teamKills = 0;
            roundsWon = 0;
            teamSpawn = new Vector3(0,0,0);
        }

        public Team(int _teamNumber, Vector3 Spawn)
        {
            teamNumber = _teamNumber;
            teamKills = 0;
            roundsWon = 0;
            teamSpawn = Spawn;
        }

        public int teamNumber;
        public List<Tank> tanks = new();
        public int teamKills { get; set; }
        public int roundsWon { get; set; }
        public Vector3 teamSpawn { get; set; }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Team.Tank AddTank(GameObject tankToAdd)
    {
        Team.Tank tank = new Team.Tank(tankToAdd);

        switch (GameSingleton.GetInstance().currentGameMode)
        {
            case GameSingleton.GameMode.DeathMatch:
                teams.Add(new Team(NewTeamNumber));
                teams[NewTeamNumber].tanks.Add(tank);
                tank.team = teams[NewTeamNumber];

                if (NetworkManager.Singleton) tank.tank.GetComponent<UnityNetworkTankMovement>().teamNumber = NewTeamNumber;
                else tank.tank.GetComponent<TankMovement>().teamNumber = NewTeamNumber;
                
                ++NewTeamNumber;

                break;
            case GameSingleton.GameMode.TeamDeathMatch:
            case GameSingleton.GameMode.Domination:
            case GameSingleton.GameMode.CaptureTheFlag:
                if (!teamsSet) SetTeams();

                if (teams[0].tanks.Count < teams[1].tanks.Count)
                {
                    teams[0].tanks.Add(tank);
                    tank.team = teams[0];

                    if (NetworkManager.Singleton) tank.tank.GetComponent<UnityNetworkTankMovement>().teamNumber = 0;
                    else tank.tank.GetComponent<TankMovement>().teamNumber = 0;
                }
                else
                {
                    teams[1].tanks.Add(tank);
                    tank.team = teams[1];

                    if (NetworkManager.Singleton) tank.tank.GetComponent<UnityNetworkTankMovement>().teamNumber = 1;
                    else tank.tank.GetComponent<TankMovement>().teamNumber = 1;
                }
                break;
            default:
                break;
        }

        return tank;
    }

    public void SetBaseLights()
    {
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");

        foreach (GameObject CTFBase in bases)
        {
            FlagBase currBase = CTFBase.GetComponent<FlagBase>();

            if (currBase.teamNumber == GameSingleton.GetInstance().playerTeam) 
                currBase.teamLight.color = Color.blue;
            else currBase.teamLight.color = Color.red;
        }
    }

    public void SetFriendEnemy()
    {
        for (ushort i = 0; i < teams.Count; ++i)
        {
            for (ushort j = 0; j < teams[i].tanks.Count; ++j)
            {
                Color color;
                if (NetworkManager.Singleton)
                {
                    UnityNetworkTankMovement tank = teams[i].tanks[j].tank.GetComponent<UnityNetworkTankMovement>();
                    if (teams[i].teamNumber == GameSingleton.GetInstance().playerTeam) color = Color.blue;
                    else color = Color.red;

                    color.a = 0.5f;
                    tank.m_FriendEnemy.color = color;
                }
                else
                {
                    TankMovement tank = teams[i].tanks[j].tank.GetComponent<TankMovement>();
                    if (teams[i].teamNumber == GameSingleton.GetInstance().playerTeam) color = Color.blue;
                    else color = Color.red;

                    color.a = 0.5f;
                    tank.m_FriendEnemy.color = color;
                }

            }
        }
    }

    public int GetOwnerTankID()
    {
        return NewOwnerID++;
    }

    private void SetTeams()
    {
        Debug.Log("Set Teams");
        //for (ushort i = 0; i < 2; ++i)
        //{
        //    teams.Add(new Team(i));
        //}
        teams.Add(new Team(1, team1Spawn));
        teams.Add(new Team(2, team2Spawn));
        teamsSet = true;
    }
}
