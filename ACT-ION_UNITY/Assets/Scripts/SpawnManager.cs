using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpawnManager : NetworkBehaviour

{
    public List<GameObject> dead = new();
    public List<float> deathTime = new();
    public float roundRestartDelay = 2f;
    public float RespawnTime = 2;
    public float NetworkDeathDelay = 0.2f;
    static public int defaultSpawnRadius = 20;
    public bool endOfRound { get; set; }
    public float roundEndTime { get; set; }


    private InfoCollector collector;
    private static List<Vector3> pos = new();
    private const int roundEndTimeNotSet = -1;

    void Awake(){
        pos.Add(new Vector3(0, 0, 34));
        pos.Add(new Vector3(-50, 0, 0));
        pos.Add(new Vector3(37, 0, 10));
        pos.Add(new Vector3(0, 0, -55));
    }

    // Start is called before the first frame update
    void Start()
    {
        collector = GetComponent<InfoCollector>();
        endOfRound = false;
        roundEndTime = roundEndTimeNotSet;


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 spawnAreaPos = new Vector3(0, 0, 0);

        // Checking if it is the end of round is in InfoCollector script

        if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamBattle)
        {
            float currTime = Time.time;
            if (!endOfRound) return;

            if (roundEndTime == roundEndTimeNotSet)
            {
                roundEndTime = currTime;
//                Debug.Log("Set round end time");
            }

            if (currTime < roundRestartDelay + roundEndTime)
            {
//              Debug.Log("On delay");
                return;
            }

//            Debug.Log("Round ended, respawning");
            foreach( InfoCollector.Team team in collector.teams)
            {
                foreach(InfoCollector.Team.TankHolder tankHolder in team.tanks)
                {
                    if (NetworkManager.Singleton) tankHolder.tank.GetComponent<UnityNetworkTankHealth>().MoveOnRespawn(GetSpawnPos(team.teamSpawn, defaultSpawnRadius));
                    else tankHolder.tank.transform.position = GetSpawnPos(team.teamSpawn, defaultSpawnRadius);

                    tankHolder.tank.SetActive(false);
                    tankHolder.tank.SetActive(true);
                }
                team.alivePlayers = team.tanks.Count;
            }

            roundEndTime = roundEndTimeNotSet;
            endOfRound = false;

            return;
        }


        if (dead.Count != deathTime.Count)
        {
            throw new Exception("Dead count not equal death time count");
        }

        if (NetworkManager.Singleton)
        {

            for (ushort i = 0; i < dead.Count; ++i)
            {
                if (dead[i].activeSelf && (Time.time > deathTime[i] + NetworkDeathDelay))
                {
                    //Debug.Log("Setting inactive");
                    dead[i].SetActive(false);
                    continue;
                }

                if (Time.time > deathTime[i] + RespawnTime)
                {
                    ///// LATER CHECK IF WORKS DIRECTLY
                    ///

                    //dead[i].transform.position = GetSpawnPos(spawnAreaPos, 30);
                    //RespawnClientRpc(dead[i], spawnPos);
                    //dead[i].transform.position = GetSpawnPos(spawnAreaPos, 30);
                    InfoCollector.Team.TankHolder tank = dead[i].GetComponent<UnityNetworkTankShooting>().tankHolder;
                    dead[i].GetComponent<UnityNetworkTankHealth>().MoveOnRespawn(GetSpawnPos(tank.team.teamSpawn, defaultSpawnRadius));
                    dead[i].SetActive(true);

                    //dead[i].transform.position = GetSpawnPos(spawnAreaPos, 30);
                    dead.RemoveAt(i);
                    deathTime.RemoveAt(i);
                    --i;
                }
            }
        }
        else
        {
            for (ushort i = 0; i < dead.Count; ++i)
            {

                if (Time.time > deathTime[i] + RespawnTime)
                {
                    InfoCollector.Team.TankHolder tank = dead[i].GetComponent<TankShooting>().tankHolder;
                    dead[i].transform.position = GetSpawnPos(tank.team.teamSpawn, defaultSpawnRadius);
                    dead[i].SetActive(true);
                    dead.RemoveAt(i);
                    deathTime.RemoveAt(i);
                    --i;
                }
            }
        }
    }

    static public Vector3 GetSpawnPos(Vector3 spawnOrigin, float spawnRadius, ushort spawnClearArea = 4)
    {
        Vector3 spawnPos;
        Collider[] collisionArray;


        //do
        //{
        //    spawnPos = pos[UnityEngine.Random.Range(0, 3)];
        //    collisionArray = Physics.OverlapBox(spawnPos, Vector3.one * spawnClearArea, Quaternion.Euler(Vector3.zero), ~0, QueryTriggerInteraction.Ignore);
        //}
        do
        {
            spawnPos = new Vector3(spawnOrigin.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius), 0,
                                   spawnOrigin.z + UnityEngine.Random.Range(-spawnRadius, spawnRadius));
            collisionArray = Physics.OverlapBox(spawnPos, Vector3.one * spawnClearArea, Quaternion.Euler(Vector3.zero), ~0, QueryTriggerInteraction.Ignore);
            //Debug.Log("Trying to get a spawnpoint");
        }
        while (collisionArray.Length != 0);

        return spawnPos;
    }
}
