using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InfoCollector : MonoBehaviour
{
    public List<GameObject> shells = new();
    public List<Team> teams = new();
    public List<GameObject> mapObjects = new();
    public string botMovement;
    public string gameResult = "Playing";

    private bool teamsSet = false;
    private static int NewTeamNumber = 0;
    private int NewOwnerID;
    public class Team
    {
        public Team(int _teamNumber)
        {
            teamNumber = _teamNumber;
        }

        public int teamNumber;
        public List<GameObject> tanks = new();
    }

    // Update is called once per frame
    void Update()
    {
        //foreach(Team team in teams)
        //{
        //    team.tanks.Sort();
        //}
    }

    public void AddTank(GameObject Tank)
    {
        switch (GameSingleton.GetInstance().currentGameMode)
        {
            case GameSingleton.GameMode.DeathMatch:
                teams.Add(new Team(NewTeamNumber));
                teams[NewTeamNumber].tanks.Add(Tank);

                if (NetworkManager.Singleton) Tank.GetComponent<UnityNetworkTankMovement>().teamNumber = NewTeamNumber;
                else Tank.GetComponent<TankMovement>().teamNumber = NewTeamNumber;
                
                ++NewTeamNumber;

                break;
            case GameSingleton.GameMode.TeamDeathMatch:
                if (!teamsSet) SetTeams();

                if (teams[0].tanks.Count < teams[1].tanks.Count)
                {
                    teams[0].tanks.Add(Tank);

                    if (NetworkManager.Singleton) Tank.GetComponent<UnityNetworkTankMovement>().teamNumber = 0;
                    else Tank.GetComponent<TankMovement>().teamNumber = 0;
                }
                else
                {
                    teams[1].tanks.Add(Tank);
                    if (NetworkManager.Singleton) Tank.GetComponent<UnityNetworkTankMovement>().teamNumber = 1;
                    else Tank.GetComponent<TankMovement>().teamNumber = 1;
                }
                break;
            default:
                break;
        }
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
                    UnityNetworkTankMovement tank = teams[i].tanks[j].GetComponent<UnityNetworkTankMovement>();
                    if (teams[i].teamNumber == GameSingleton.GetInstance().playerTeam) color = Color.blue;
                    else color = Color.red;

                    color.a = 0.5f;
                    tank.m_FriendEnemy.color = color;
                }
                else
                {
                    TankMovement tank = teams[i].tanks[j].GetComponent<TankMovement>();
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
        for (ushort i = 0; i < 2; ++i)
        {
            teams.Add(new Team(i));
        }
        teamsSet = true;
    }
}
