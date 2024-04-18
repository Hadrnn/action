using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;

public class UnityNetworkTankShooting : NetworkBehaviour
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

    public float cooldown = 1f;
    private float ShootTime = 0f;

    private string m_FireButton;                // The input axis that is used for launching shells.
    private float m_CurrentLifeTime;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.


    private void OnEnable()
    {
        m_CurrentLifeTime = m_MinLifeTime;
    }


    private void Start()
    {
        if (!IsOwner) return;


        m_FireButton = "Fire";

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLifeTime - m_MinLifeTime) / m_MaxChargeTime;
    }


    private void Update()
    {

        if (!IsOwner) return;

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (m_CurrentLifeTime >= m_MaxLifeTime && !m_Fired)
        {
            // ... use the max force and launch the shell.
            m_CurrentLifeTime = m_MaxLifeTime;
            Fire();
        }
        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown(m_FireButton))
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLifeTime = m_MinLifeTime;

            // Change the clip to the charging clip and start it playing.
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Increment the launch force and update the slider.
            m_CurrentLifeTime += m_ChargeSpeed * Time.deltaTime;

            //m_AimSlider.value = m_CurrentLifeTime;
        }
        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // ... launch the shell.
            Fire();
        }
    }


    private void Fire()
    {
        float CurrentTime = Time.time;
        if ((CurrentTime - ShootTime) < cooldown)
        {
            return;
        }
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;

        FireServerRpc();

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ShootTime = Time.time;
    }

    [ServerRpc]
    public void FireServerRpc()
    {        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        NetworkObject new_shell = shellInstance.GetComponent<NetworkObject>();

        new_shell.Spawn();
        //new_shell.RemoveOwnership();

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        UnityNetworkShellExplosion explosion = shellInstance.GetComponent<UnityNetworkShellExplosion>();
        explosion.m_MaxLifeTime = m_CurrentLifeTime;

        explosion.teamNumber = GetComponent<UnityNetworkTankMovement>().teamNumber;
        explosion.OwnerTankID = GetComponent<UnityNetworkTankMovement>().GetOwnerTankID();

        m_CurrentLifeTime = m_MinLifeTime;
        shellInstance.velocity = m_Velocity * m_FireTransform.forward;
    }

}