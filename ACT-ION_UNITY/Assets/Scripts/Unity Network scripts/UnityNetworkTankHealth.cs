using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class UnityNetworkTankHealth : NetworkBehaviour, IHealth
{
    public float m_StartingHealth = 100f;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public GameObject m_ExplosionPrefab;
    public bool m_Dead { get; private set; }

    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    private NetworkVariable<float> m_CurrentHealth;
                           

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_CurrentHealth = new NetworkVariable<float>(m_StartingHealth);

        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
        m_Slider.maxValue = m_StartingHealth;
    }


    private void OnEnable()
    {
        if (IsOwner)
        {
            GameObject cameraRig = GameObject.Find("CameraRig");
            CameraFollower follower = cameraRig.GetComponent<CameraFollower>();
            follower.m_Target = transform;
        }

        m_Dead = false;
        m_Slider.maxValue = m_StartingHealth;
        if(IsServer) m_CurrentHealth.Value = m_StartingHealth;

        SetHealthUI(m_StartingHealth);
    }

    public void heal(float amount)
    {
        Debug.LogWarning("DID NOT COMPLETE NETWORK HEAL");
    }

    public void TakeDamage(float amount, InfoCollector.Team.TankHolder shellOwner)
    {

        m_CurrentHealth.Value -= amount;
        SetHealthUIClientRpc(m_CurrentHealth.Value);
        SetHealthUI(m_CurrentHealth.Value);


        
        if (m_CurrentHealth.Value <= 0f && !m_Dead)
        {
            OnDeathClientRpc(shellOwner.team.teamNumber, shellOwner.tankID);
            OnDeath(shellOwner.team.teamNumber, shellOwner.tankID);
        }
    }

    public bool IsDead()
    {
        return m_CurrentHealth.Value <= 0f;
    }

    private void SetHealthUI(float currentHealth)
    {

        m_Slider.value = currentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, currentHealth / m_StartingHealth);
    }

    [ClientRpc]
    private void SetHealthUIClientRpc(float currentHealth)
    {
        SetHealthUI(currentHealth);
    }

    [ClientRpc]
    private void OnDeathClientRpc(int killerTeamNumber, ulong killerTankID)
    {
        OnDeath(killerTeamNumber, killerTankID);
    }

    public void MoveOnRespawn(Vector3 position)
    {
        if (!IsOwner) return;

        //Debug.Log("Im trying to move");
        //Debug.Log(position);
        //gameObject.GetComponent<Rigidbody>().MovePosition(position);
        transform.position = position;
        
    }
    private void OnDeath(int killerTeamNumber, ulong killerTankID)
    {
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        InfoCollector collector = GetComponent<UnityNetworkTankMovement>().collector;
        SpawnManager manager = collector.GetComponent<SpawnManager>();


        manager.dead.Add(gameObject);
        manager.deathTime.Add(Time.time);

        InfoCollector.Team.TankHolder holder = GetComponent<UnityNetworkTankShooting>().tankHolder;
        ++holder.deaths;
        --holder.team.alivePlayers;


        InfoCollector.Team.TankHolder shellOwner = null;


        foreach (InfoCollector.Team.TankHolder tankHolder in collector.teams[killerTeamNumber].tanks)
        {
            if (tankHolder.tankID == killerTankID)
            {
                //Debug.Log("FOUND HIM");
                shellOwner = tankHolder;
                break;
            }
        }


        if ((shellOwner != null) && (shellOwner != holder)) 
        {
            ++shellOwner.kills;
            ++shellOwner.team.teamKills;
        }

        if (IsServer)
        {
            UnityNetworkFlagCapture flag = GetComponentInChildren<UnityNetworkFlagCapture>();
            if (flag)
            {
                Debug.Log("I have a flag while dying");
                flag.transform.SetParent(null);
                flag.SetParentClientRpc(0, 0, true, false);
                flag.SetCaptured(false);
            }
            return;
        }

        if (IsOwner)
        {
            GameObject cameraRig = GameObject.Find("CameraRig");
            CameraFollower follower = cameraRig.GetComponent<CameraFollower>();
            follower.m_Target = null;
        }


        Vector3 Grave = new Vector3(transform.position.x,-10, transform.position.z);

        transform.position = Grave;

        // Tank is turned off in spawn manager;
        //gameObject.SetActive(false);
    }
}
