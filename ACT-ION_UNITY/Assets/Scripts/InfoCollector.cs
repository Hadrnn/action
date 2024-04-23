using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class InfoCollector : NetworkBehaviour
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
    private static int NewOwnerID = 0;

    public class Team
    {
        public class TankHolder
        {
            public TankHolder(GameObject _tank)
            {
                tank = _tank;
                kills = 0;
                deaths = 0;
                tankID = GetOwnerTankID();
            }

            public GameObject tank;
            public int kills;
            public int deaths;
            public Team team;
            public int tankID;
        }
        public Team(int _teamNumber)
        {
            teamNumber = _teamNumber;
            teamKills = 0;
            roundsWon = 0;
            alivePlayers = 0;
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
        public List<TankHolder> tanks = new();
        public int alivePlayers { get; set; }
        public int teamKills { get; set; }
        public int roundsWon { get; set; }
        public Vector3 teamSpawn { get; set; }

    }

    // Update is called once per frame
    void Update()
    {
        if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamBattle)
        {
            if (GetComponent<SpawnManager>().endOfRound) return;

            if (teams[0].alivePlayers == 0)
            {
                ++teams[1].roundsWon;
                Debug.Log("Team 1 won");
                GetComponent<SpawnManager>().endOfRound = true;
            }
            else if (teams[1].alivePlayers == 0)
            {
                Debug.Log("Team 0 won");
                ++teams[0].roundsWon;
                GetComponent<SpawnManager>().endOfRound = true;
            }
        }
    }

    public Team.TankHolder AddTank(GameObject tankToAdd)
    {
        Team.TankHolder tankHolder = new Team.TankHolder(tankToAdd);

        switch (GameSingleton.GetInstance().currentGameMode)
        {
            case GameSingleton.GameMode.DeathMatch:
                teams.Add(new Team(NewTeamNumber));
                teams[NewTeamNumber].tanks.Add(tankHolder);
                tankHolder.team = teams[NewTeamNumber];
                ++tankHolder.team.alivePlayers;

                if (NetworkManager.Singleton) tankHolder.tank.GetComponent<UnityNetworkTankMovement>().teamNumber = NewTeamNumber;
                else tankHolder.tank.GetComponent<TankMovement>().teamNumber = NewTeamNumber;
                
                ++NewTeamNumber;

                break;
            case GameSingleton.GameMode.TeamDeathMatch:
            case GameSingleton.GameMode.Domination:
            case GameSingleton.GameMode.CaptureTheFlag:
            case GameSingleton.GameMode.TeamBattle:
                if (!teamsSet) SetTeams();

                if (teams[0].tanks.Count < teams[1].tanks.Count)
                {
                    teams[0].tanks.Add(tankHolder);
                    tankHolder.team = teams[0];

                    if (NetworkManager.Singleton) tankHolder.tank.GetComponent<UnityNetworkTankMovement>().teamNumber = 0;
                    else tankHolder.tank.GetComponent<TankMovement>().teamNumber = 0;
                }
                else
                {
                    teams[1].tanks.Add(tankHolder);
                    tankHolder.team = teams[1];

                    if (NetworkManager.Singleton) tankHolder.tank.GetComponent<UnityNetworkTankMovement>().teamNumber = 1;
                    else tankHolder.tank.GetComponent<TankMovement>().teamNumber = 1;
                }
                break;
            default:
                break;
        }

        ++tankHolder.team.alivePlayers;
        return tankHolder;
    }

    public void SetBaseLights()
    {
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");

        if (NetworkManager.Singleton)
        {
                foreach (GameObject CTFBase in bases)
                {
                    UnityNetworkFlagBase currBase = CTFBase.GetComponent<UnityNetworkFlagBase>();

                    if (currBase.teamNumber.Value == GameSingleton.GetInstance().playerTeam)
                        currBase.teamLight.color = Color.blue;
                    else currBase.teamLight.color = Color.red;
                }
        }
        else
        {

            foreach (GameObject CTFBase in bases)
            {
                FlagBase currBase = CTFBase.GetComponent<FlagBase>();

                if (currBase.teamNumber == GameSingleton.GetInstance().playerTeam)
                    currBase.teamLight.color = Color.blue;
                else currBase.teamLight.color = Color.red;
            }
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
                    //Debug.Log(teams[i].teamNumber);
                    TankMovement tank = teams[i].tanks[j].tank.GetComponent<TankMovement>();
                    if (teams[i].teamNumber == GameSingleton.GetInstance().playerTeam) color = Color.blue;
                    else color = Color.red;

                    color.a = 0.5f;
                    tank.m_FriendEnemy.color = color;
                }

            }
        }
    }

    public static int GetOwnerTankID()
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
        teams.Add(new Team(0, team1Spawn));
        teams.Add(new Team(1, team2Spawn));
        teamsSet = true;
    }
}
