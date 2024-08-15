using Unity.Netcode;
using UnityEngine;

public class BonusSpawner : NetworkBehaviour
{

    public float spawnRadius = 30f;

    public GameObject[] bonuses;
    public static int[] bonusCounters;
    public float[] respawnTimes;
    public int[] maxCounts;
    private float[] lastSpawnTimes;


    // Start is called before the first frame update
    void Awake()
    {
        bonusCounters = new int[bonuses.Length];
        lastSpawnTimes = new float[bonuses.Length];

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (NetworkManager.Singleton && !IsServer) return;


        for(int i = 0; i< bonuses.Length; ++i)
        {

            if (bonusCounters[i] < maxCounts[i] &&
                Time.time - lastSpawnTimes[i] > respawnTimes[i])
            {
                GameObject bonus = Instantiate(bonuses[i],
                    SpawnManager.GetSpawnPos(Vector3.zero, spawnRadius),
                    Quaternion.Euler(Vector3.zero));

                if (NetworkManager.Singleton)
                {
                    bonus.GetComponent<NetworkObject>().Spawn();
                }

                lastSpawnTimes[i] = Time.time;
            }
        }
    }
}
