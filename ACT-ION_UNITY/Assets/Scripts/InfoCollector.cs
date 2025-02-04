using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class InfoCollector : NetworkBehaviour
{
    public List<GameObject> shells = new();
    public List<Team> teams = new();
    public List<GameObject> mapObjects = new();
    public List<GameObject> objectives = new();

    public string botMovement;
    public string gameResult = "Playing";

    public Vector3 team1Spawn { get; set; }
    public Vector3 team2Spawn { get; set; }

    private bool teamsSet = false;
    private static int NewTeamNumber = 0;
    private static ulong NewOwnerID = 0;

    public class Team
    {
        public class TankHolder : IComparable<TankHolder>
        {
            public TankHolder(GameObject _tank, string _name)
            {
                tank = _tank;
                name = _name;
                kills = 0;
                deaths = 0;
                if (NetworkManager.Singleton)
                {
                    tankID = _tank.GetComponent<UnityNetworkTankHealth>().OwnerClientId;
                }
                else
                {
                    tankID = GetOwnerTankID();
                }
                //Debug.Log("TANK ID (CLIENT ID) " + tankID.ToString());
            }

            public GameObject tank;
            public int kills;
            public int deaths;
            public Team team;
            public ulong tankID;
            public string name;

            public int CompareTo(TankHolder other)
            {
                return kills.CompareTo(other.kills);
            }
        }
        public Team(int _teamNumber)
        {
            teamNumber = _teamNumber;
            teamKills = 0;
            teamStat = 0;
            alivePlayers = 0;
            teamSpawn = new Vector3(0,0,0);
        }

        public Team(int _teamNumber, int _teamKills, float _teamStat, int _alivePlayers)
        {
            teamNumber = _teamNumber;
            teamKills = _teamKills;
            teamStat = _teamStat;
            alivePlayers = _alivePlayers;
            teamSpawn = new Vector3(0, 0, 0);
        }

        public Team(int _teamNumber, Vector3 Spawn)
        {
            teamNumber = _teamNumber;
            teamKills = 0;
            teamStat = 0;
            teamSpawn = Spawn;
        }

        public int teamNumber;
        public List<TankHolder> tanks = new();

        public int alivePlayers { get; set; }
        public int teamKills { get; set; }
        public float teamStat { get; set; }
        public Vector3 teamSpawn { get; set; }
    }

    private void Start()
    {
        NewTeamNumber = 0;
    }

    void Update()
    {

        if (GameSingleton.GetInstance().currentGameMode != GameSingleton.GameMode.DeathMatch
            && !teamsSet)
        {
            //throw new Exception("Did not set teams");
            Debug.Log("Did not set teams");
            return;
        }

        //teams[0].tanks.Sort(delegate (Team.TankHolder first, Team.TankHolder second) { return first.kills.CompareTo(second.kills); });
        for (int i = 0; i < teams.Count; ++i)
        {
            teams[i].tanks.Sort();
        }

        if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamBattle)
        {
            if (GetComponent<SpawnManager>().endOfRound) return;

            if (teams[0].alivePlayers == 0)
            {
                ++teams[1].teamStat;
                Debug.Log("Team 1 won");
                GetComponent<SpawnManager>().endOfRound = true;
            }
            else if (teams[1].alivePlayers == 0)
            {
                Debug.Log("Team 0 won");
                ++teams[0].teamStat;
                GetComponent<SpawnManager>().endOfRound = true;
            }
        }
    }

    public Team.TankHolder AddTank(GameObject tankToAdd, string name = "NO NAME")
    {
        Team.TankHolder tankHolder = new Team.TankHolder(tankToAdd, name);

        switch (GameSingleton.GetInstance().currentGameMode)
        {
            case GameSingleton.GameMode.DeathMatch:
                teams.Add(new Team(NewTeamNumber));
                teams[NewTeamNumber].tanks.Add(tankHolder);
                tankHolder.team = teams[NewTeamNumber];
                ++tankHolder.team.alivePlayers;

                if (!NetworkManager.Singleton) tankHolder.tank.GetComponent<TankMovement>().teamNumber = NewTeamNumber;

                ++NewTeamNumber;

                break;
            case GameSingleton.GameMode.TeamDeathMatch:
            case GameSingleton.GameMode.Domination:
            case GameSingleton.GameMode.CaptureTheFlag:
            case GameSingleton.GameMode.TeamBattle:

                if (!teamsSet) SetTeams();

                if(!NetworkManager.Singleton && 
                    tankHolder.tank.GetComponent<TankMovement>().teamNumber != TankMovement.teamNotSet)
                {
                    int teamNumber = tankHolder.tank.GetComponent<TankMovement>().teamNumber;

                    //Debug.Log("Spawning a tank with a pre-set team:" + teamNumber.ToString());

                    if (teamNumber != 0 && teamNumber != 1) throw new Exception("Invalid team number pre-set");

                    teams[teamNumber].tanks.Add(tankHolder);
                    tankHolder.team = teams[teamNumber];


                    tankHolder.tank.GetComponent<TankMovement>().teamNumber = teamNumber;

                    ++tankHolder.team.alivePlayers;
                    return tankHolder;
                }

                if (teams[0].tanks.Count < teams[1].tanks.Count)
                {
                    teams[0].tanks.Add(tankHolder);
                    tankHolder.team = teams[0];

                    if (!NetworkManager.Singleton)
                            tankHolder.tank.GetComponent<TankMovement>().teamNumber = 0;
                }
                else
                {
                    teams[1].tanks.Add(tankHolder);
                    tankHolder.team = teams[1];

                    if (!NetworkManager.Singleton)
                        tankHolder.tank.GetComponent<TankMovement>().teamNumber = 1;
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
                if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag)
                {
                    UnityNetworkFlagBase currBase = CTFBase.GetComponent<UnityNetworkFlagBase>();

                    if (currBase.teamNumber.Value == GameSingleton.GetInstance().playerTeam)
                        currBase.teamLight.color = Color.blue;
                    else currBase.teamLight.color = Color.red;

                }
                else if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.Domination)
                {
                    UnityNetworkBaseCapture currBase = CTFBase.GetComponent<UnityNetworkBaseCapture>();

                    Color sliderColor;


                    if (currBase.occupantTeam.Value == GameSingleton.GetInstance().playerTeam)
                    {
                        currBase.teamLight.color = Color.blue;
                        sliderColor = Color.blue;

                    }
                    else
                    {
                        currBase.teamLight.color = Color.red;
                        sliderColor = Color.red;
                    }

                    sliderColor.a = 0.5f;

                    currBase.CaptureImage.color = sliderColor;
                }
            }
        }
        else
        {

            foreach (GameObject CTFBase in bases)
            {
                if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag)
                {
                    FlagBase currBase = CTFBase.GetComponent<FlagBase>();

                    if (currBase.teamNumber == GameSingleton.GetInstance().playerTeam)
                        currBase.teamLight.color = Color.blue;
                    else currBase.teamLight.color = Color.red;
                }
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

    private static ulong GetOwnerTankID()
    {
        return NewOwnerID++;
    }

    private void SetTeams()
    {
        //Debug.Log("Set Teams");
        //for (ushort i = 0; i < 2; ++i)
        //{
        //    teams.Add(new Team(i));
        //}
        teams.Add(new Team(0, team1Spawn));
        teams.Add(new Team(1, team2Spawn));
        teamsSet = true;
    }

}
