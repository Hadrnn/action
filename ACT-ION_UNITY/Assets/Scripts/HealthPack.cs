using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int health = 50;
    public int baseHeight = 2;
    public float upDownSpeed = 2f;
    public float rotationSpeed = 0.3f;


    void Start()
    {
        ++BonusSpawner.healthPackCounter;
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

        IHealth colliderHealth = other.GetComponent<IHealth>();
        if (colliderHealth == null) return;

        colliderHealth.heal(health);
        --BonusSpawner.healthPackCounter;
        Destroy(gameObject);

    }

}
