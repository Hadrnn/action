using Unity.Netcode;
using UnityEngine;
using static GameSingleton;

public class HealthPack : NetworkBehaviour
{
    public int health = 50;
    public int baseHeight = 2;
    public float upDownSpeed = 2f;
    public float rotationSpeed = 0.3f;


    void Start()
    {
        ++BonusSpawner.bonusCounters[GameSingleton.BonusIndex.HealthPack]; ;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = baseHeight + Mathf.Sin(upDownSpeed * Time.time);
        transform.position = pos;

        Vector3 turn = transform.rotation.eulerAngles;
        turn.y += rotationSpeed;
        transform.rotation = Quaternion.Euler(turn);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.Singleton && !IsServer)
        {
            return;
        }

        IHealth colliderHealth = other.GetComponent<IHealth>();
        if (colliderHealth == null) return;

        colliderHealth.heal(health);
        --BonusSpawner.bonusCounters[GameSingleton.BonusIndex.HealthPack];


        if (NetworkManager.Singleton && IsServer)
        {
            NetworkObject healthPack = gameObject.GetComponent<NetworkObject>();
            healthPack.Despawn();
        }
        else
        {
            Destroy(gameObject);
        }

    }

}
