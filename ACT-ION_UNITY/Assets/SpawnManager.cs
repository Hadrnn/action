using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpawnManager : NetworkBehaviour

{
    public List<GameObject> dead = new();
    public List<float> deathTime = new();

    public float RespawnTime = 2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 spawnAreaPos = new Vector3(0, 0, 0);
        if (dead.Count != deathTime.Count)
        {
            throw new Exception("Dead count not equal death time count");
        }

        if (NetworkManager.Singleton)
        {
            if (IsClient)
            {
                for (ushort i = 0; i < dead.Count; ++i)
                {

                    if (Time.time > deathTime[i] + RespawnTime)
                    {
                        ///// LATER CHECK IF WORKS DIRECTLY
                        Vector3 spawnPos = GetSpawnPos(spawnAreaPos, 30);
                        //RespawnClientRpc(dead[i], spawnPos);
                        //dead[i].transform.position = GetSpawnPos(spawnAreaPos, 30);
                        
                        dead[i].SetActive(true);
                        dead.RemoveAt(i);
                        deathTime.RemoveAt(i);
                        --i;
                    }
                }
            }
        }
        else
        {
            for (ushort i = 0; i < dead.Count; ++i)
            {

                if (Time.time > deathTime[i] + RespawnTime)
                {
                    dead[i].transform.position = GetSpawnPos(spawnAreaPos, 30);
                    dead[i].SetActive(true);
                    dead.RemoveAt(i);
                    deathTime.RemoveAt(i);
                    --i;
                }
            }
        }
    }

    static Vector3 GetSpawnPos(Vector3 spawnOrigin, float spawnRadius, ushort spawnClearArea = 4)
    {
        Vector3 spawnPos;
        Collider[] collisionArray;

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
