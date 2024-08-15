using Unity.Networking.Transport.Error;
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{

    public int spawnRadius = 10;

    public GameObject healthPack;
    public static int healthPackCounter = 0;
    public float healthPackRespawnTime = 10;
    public const int healthPackMaxCount = 5;
    private float lastHPSpawnTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        healthPackCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
