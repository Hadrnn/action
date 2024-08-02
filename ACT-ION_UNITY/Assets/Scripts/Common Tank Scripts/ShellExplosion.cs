using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem m_ExplosionParticles;         
    public AudioSource m_ExplosionAudio;
    public ParticleSystem DustTrail;
    public float m_MaxDamage = 100f;
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 1f;
    public float m_ExplosionRadius = 5f;

    public InfoCollector.Team.TankHolder owner { get; set; }

    protected float startTime;

    private void Start()
    {
        startTime = Time.time;
    }


    protected virtual void Explode()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {

            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;


            if (!GameSingleton.GetInstance().friendlyFire && targetRigidbody.GetComponent<TankMovement>().teamNumber == owner.team.teamNumber) continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage, owner);
        }
        m_ExplosionParticles.transform.parent = null;
        DustTrail.transform.parent = null;


        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();


        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(DustTrail.gameObject, DustTrail.main.duration);

        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.shells.Remove(gameObject);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FlagCapture>() ||
            other.GetComponent<FlagBase>() ||
            other.GetComponent<BaseCapture>() ||
            other.GetComponent<HealthPack>()) return;

        Explode();
    }


    private void FixedUpdate()
    {
        if (Time.time > startTime + m_MaxLifeTime)
        {
            Explode();
        }
    }

    protected float CalculateDamage(Vector3 targetPosition)
    {

        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}