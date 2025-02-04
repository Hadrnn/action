﻿using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.


    private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
    private float m_CurrentHealth;                      // How much health the tank currently has.
    private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?


    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive(false);
        m_Slider.maxValue = m_StartingHealth;
    }


    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Update the health slider's value and color.
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
        // Set the slider's value appropriately.
        m_Slider.value = m_CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath(InfoCollector.Team.TankHolder shellOwner)
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