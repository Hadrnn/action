using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotAPCShooting : MonoBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_Velocity = 30f;
    public float m_MinLifeTime = 1f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLifeTime = 2f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.
    public bool onReload = false;

    public float cooldown = 1f;
    private float ShootTime = 0f;

    private float m_CurrentLifeTime;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.

    private BotAPCMovement Body;
    // How fast the launch force increases, based on the max charge time.

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentLifeTime = m_MinLifeTime;
        Body = GetComponent<BotAPCMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float CurrentTime = Time.time;
        if ((CurrentTime - ShootTime) > cooldown)
        {
            onReload = false;
        }
    }

    public void Fire()
    {
        float CurrentTime = Time.time;
        if ((CurrentTime - ShootTime) < cooldown)
        {
            return;
        }
        // Set the fired flag so only Fire is only called once.
        onReload = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Add shell object to InfoCollector
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.shells.Add(shellInstance.GameObject());

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_Velocity * m_FireTransform.forward; ;

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ShellExplosion explosion = shellInstance.GetComponent<ShellExplosion>();
        explosion.m_MaxLifeTime = m_CurrentLifeTime;
        m_CurrentLifeTime = m_MinLifeTime;
        ShootTime = Time.time;

        Body.counter = BotAPCMovement.discret;
    }
}
