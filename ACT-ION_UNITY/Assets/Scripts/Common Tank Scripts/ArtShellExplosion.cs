using UnityEngine;

public class ArtShellExplosion : ShellExplosion
{
    public Vector3 forward;
    public float start_angle;
    public ArtShooting tank;
    public float velocity = 40f;
    public float g = 20f;

    private Rigidbody m_Rigidbody;
    private float up_speed;

    private void Start()
    {
        startTime = Time.time;
        m_Rigidbody = GetComponent<Rigidbody>();
        forward *= Mathf.Cos(Mathf.Deg2Rad * start_angle);
        forward.y = Mathf.Sin(Mathf.Deg2Rad * start_angle);
        up_speed = velocity * Mathf.Sin(Mathf.Deg2Rad * start_angle);
    }


    protected override void Explode()
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
            targetHealth.TakeDamage(damage, owner);
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


        //GameObject cameraRig = GameObject.Find("CameraRig");
        //CameraFollower follower = cameraRig.GetComponent<CameraFollower>();
        //follower.m_Target = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FlagCapture>() || other.GetComponent<FlagBase>()) return;

        if (other.gameObject.GetComponent<ArtShooting>() != null)
        {
            if (other.gameObject.GetComponent<ArtShooting>() == tank)
            {

                return;
            }
        }
        Explode();
    }


    private void FixedUpdate()
    {
        if (m_Rigidbody.position.y < 0.5)
        {
            Explode();
        }
        Vector3 movement = Vector3.zero;
        up_speed = up_speed - g*Time.deltaTime;
        movement.x = velocity*Time.deltaTime * forward.x;
        movement.z = velocity * Time.deltaTime * forward.z;
        movement.y = up_speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        Vector3 speed_forward = movement.normalized;
        Vector3 speed_forward_another_axis = new Vector3(speed_forward.y, 0, Mathf.Sqrt(speed_forward.x * speed_forward.x + speed_forward.z * speed_forward.z));
        Vector3 forward_another_axis = new Vector3(transform.forward.y, 0, Mathf.Sqrt(transform.forward.x * transform.forward.x + transform.forward.z * transform.forward.z));
        Vector3 Axis = new Vector3(0, 1, 0);
        float turn = -Vector3.SignedAngle(forward_another_axis, speed_forward_another_axis, Axis);
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
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