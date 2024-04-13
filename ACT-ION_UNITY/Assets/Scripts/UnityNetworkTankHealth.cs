using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UnityNetworkTankHealth : NetworkBehaviour
{
    public int playerNumber = 1;
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.


    private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
    private NetworkVariable<float> m_CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?

    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_CurrentHealth = new NetworkVariable<float>(m_StartingHealth);

        // Get a reference to the audio source on the instantiated prefab.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive(false);
        m_Slider.maxValue = m_StartingHealth;
    }


    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        m_Dead = false;
        m_Slider.maxValue = m_StartingHealth;
        if(IsServer) m_CurrentHealth.Value = m_StartingHealth;

        SetHealthUI(m_StartingHealth);
    }

    private void Update()
    {
        //SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth.Value -= amount;
        // Change the UI elements appropriately.
        SetHealthUIClientRpc(m_CurrentHealth.Value);
        //SetHealthUI();

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (m_CurrentHealth.Value <= 0f && !m_Dead)
        {
            OnDeathClientRpc();
            OnDeath();
        }
    }

    public bool IsDead()
    {
        return m_CurrentHealth.Value <= 0f;
    }

    private void SetHealthUI(float currentHealth)
    {
        Debug.Log("SetHealthUI (network value/passed value)");
        // Set the slider's value appropriately.
        Debug.Log(m_CurrentHealth.Value);
        Debug.Log(currentHealth);

        m_Slider.value = currentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, currentHealth / m_StartingHealth);
    }

    [ClientRpc]
    private void SetHealthUIClientRpc(float currentHealth)
    {
        SetHealthUI(currentHealth);
    }

    [ClientRpc]
    private void OnDeathClientRpc()
    {
        OnDeath();
    }

    public void MoveOnRespawn(Vector3 position)
    {
        if (!IsOwner) return;

        Debug.Log("Im trying to move");
        Debug.Log(position);
        //gameObject.GetComponent<Rigidbody>().MovePosition(position);
        transform.position = position;
        
    }
    private void OnDeath()
    {
        // Set the flag so that this function is only called once.
        m_Dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();

        SpawnManager manager = GameObject.Find("InfoCollector").GetComponent<SpawnManager>();
        manager.dead.Add(gameObject);
        manager.deathTime.Add(Time.time);

        Vector3 Grave = new Vector3(transform.position.x,-10, transform.position.z);
        //transform.position = Grave;
        GetComponent<Rigidbody>().MovePosition(Grave);
        // Turn the tank off.
        gameObject.SetActive(false);
    }
}