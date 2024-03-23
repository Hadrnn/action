using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCollector : MonoBehaviour
{


    public List<GameObject> shells = new();
    public List<Team> teams = new();
    public List<GameObject> mapObjects = new();
    public string botMovement;
    public string gameResult = "Playing";

    public class Team
    {
        public Team(int _teamNumber)
        {
            teamNumber = _teamNumber;
        }

        public int teamNumber;
        public List<GameObject> tanks = new();
    }

    void Start()
    {
        for (int i = 0; i < 2; ++i)
        {
            teams.Add(new Team(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log((shells.Count, tanks.Count, mapObjects.Count, botMovement));
    }
}
