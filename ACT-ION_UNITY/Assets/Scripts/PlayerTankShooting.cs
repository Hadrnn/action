using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerTankShooting : TankShooting
{
    private string m_FireButton;                // The input axis that is used for launching shells.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.


    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        m_CurrentLifeTime = m_MinLifeTime;

        //m_AimSlider.value = m_MinLifeTime;
    }


    private void Start()
    {
         m_FireButton = "Fire";

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLifeTime - m_MinLifeTime) / m_MaxChargeTime;
    }


    private void Update()
    {
        // The slider should have a default value of the minimum launch force.
        //m_AimSlider.value = m_MinLifeTime;

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

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;


        // Add shell object to InfoCollector
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.shells.Add(shellInstance.GameObject());

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_Velocity * m_FireTransform.forward;

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ShellExplosion explosion = shellInstance.GetComponent<ShellExplosion>();
        explosion.m_MaxLifeTime = m_CurrentLifeTime;
        explosion.owner = tank;

        m_CurrentLifeTime = m_MinLifeTime;
        ShootTime = Time.time;
    }
}