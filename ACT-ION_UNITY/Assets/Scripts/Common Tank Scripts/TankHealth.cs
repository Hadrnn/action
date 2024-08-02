using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour, IHealth
{
    public float m_StartingHealth = 100f;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public GameObject m_ExplosionPrefab;


    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    private float m_CurrentHealth;
    private bool m_Dead;


    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
        m_Slider.maxValue = m_StartingHealth;
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    public void heal(float amount)
    {
        m_CurrentHealth += amount;
        if(m_CurrentHealth > m_StartingHealth)
        {
            m_CurrentHealth = m_StartingHealth;
        }

        SetHealthUI();
    }
    public void TakeDamage(float amount, InfoCollector.Team.TankHolder shellOwner)
    {
        m_CurrentHealth -= amount;

        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            // FOR NEURAL BOT LEARNING
            InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
            if (gameObject.GetComponent<NeuralTankMovement>()) collector.gameResult = "LOSE";
            else collector.gameResult = "WIN";


            //if (playerNumber == 0) collector.gameResult = "WIN";
            //else collector.gameResult = "LOSE";
            OnDeath(shellOwner);
        }
    }


    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath(InfoCollector.Team.TankHolder shellOwner)
    {
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        InfoCollector collector = TankMovement.collector;
        SpawnManager manager = collector.GetComponent<SpawnManager>();


        manager.dead.Add(gameObject);
        manager.deathTime.Add(Time.time);

        InfoCollector.Team.TankHolder holder = GetComponent<TankShooting>().tankHolder;
        ++holder.deaths;
        --holder.team.alivePlayers;

        if (shellOwner != holder)
        {
            ++shellOwner.kills;
            //Debug.Log(shellOwner.kills);
            ++shellOwner.team.teamKills;
        }

        FlagCapture flag = GetComponentInChildren<FlagCapture>();
        if (flag)
        {
            //Debug.Log("I have a flag while dying");
            flag.transform.SetParent(null);
            flag.IsCaptured = false;
        }

        gameObject.SetActive(false);
    }
}