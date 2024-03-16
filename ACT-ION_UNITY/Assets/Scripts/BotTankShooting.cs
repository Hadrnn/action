using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotTankShooting : MonoBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

    public float cooldoun = 1f;
    private float ShootTime = 0f;


    private string m_FireButton;                // The input axis that is used for launching shells.
    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        float CurrentTime = Time.time;
        if ( (CurrentTime - ShootTime) < cooldoun )
        {
            return;
        }
        
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Add shell object to InfoCollector
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.shells.Add(shellInstance.GameObject());

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; ;

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;
        ShootTime = Time.time;
    }
}
