using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class InfoCollector : MonoBehaviour
{
    public List<GameObject> shells = new();
    public List<Team> teams = new();
    public List<GameObject> mapObjects = new();
    public string botMovement;
    public string gameResult = "Playing";

    private static int NewTeamNumber = 0;
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
        // Debug.Log((shells.Count, tanks.Count, mapObjects.Count, botMovement));
    }

    public void AddTank(GameObject Tank)
    {
        switch (GameSingleton.GetInstance().currentMode)
        {
            case GameSingleton.GameMode.DeathMatch:
                teams.Add(new Team(NewTeamNumber));
                teams[NewTeamNumber].tanks.Add(Tank);
                Tank.GetComponent<TankMovement>().SetTeamNumber(NewTeamNumber);
                ++NewTeamNumber;
                break;
            default:
                break;
        }


    }
}
