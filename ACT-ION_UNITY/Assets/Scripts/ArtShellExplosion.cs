using UnityEngine;

public class ArtShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
    public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float m_MaxLifeTime = 1f;                    // The time in seconds before the shell is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.
    public Vector3 forward;
    public float start_angle;
    public PlayerArtShooting tank;
    public float velocity = 40f;
    public float g = 20f;

    private float startTime;
    private Rigidbody m_Rigidbody;
    private float up_speed;

    private void Start()
    {
        startTime = Time.time;
        m_Rigidbody = GetComponent<Rigidbody>();
        forward *= Mathf.Cos(Mathf.Deg2Rad*start_angle);
        forward.y = Mathf.Sin(Mathf.Deg2Rad * start_angle);
        up_speed = velocity * Mathf.Sin(Mathf.Deg2Rad * start_angle);
/*        Debug.Log(start_angle);
        Debug.Log(up_speed);*/
    }


    private void Explode()
    {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            float damage = CalculateDamage(targetRigidbody.position);

            // Deal this damage to the tank.
            targetHealth.TakeDamage(damage);
        }
        // Unparent the particles from the shell.
        m_ExplosionParticles.transform.parent = null;

        // Play the particle system.
        m_ExplosionParticles.Play();

        // Play the explosion sound effect.
        m_ExplosionAudio.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        // Remove shell object from InfoCollector
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.shells.Remove(gameObject);
        // Destroy the shell.
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerArtShooting>() != null)
        {
            if (other.gameObject.GetComponent<PlayerArtShooting>() == tank)
            {
                Debug.Log("Im in myself");
                return;
            }
        }
        Explode();
    }


    private void FixedUpdate()
    {
        if (m_Rigidbody.position.y < 0)
        {
            Explode();
        }
        Vector3 movement = Vector3.zero;
        up_speed = up_speed - g*Time.deltaTime;
        movement.x = velocity*Time.deltaTime * forward.x;
        movement.z = velocity * Time.deltaTime * forward.z;
        movement.y = up_speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
 /*       Debug.Log(forward);*/
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}