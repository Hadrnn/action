using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;
using static InfoCollector.Team;

public class PlayerAPCShooting : TankShooting
{

    private string m_FireButton;                // The input axis that is used for launching shells.
    public float birst_delay_time = 0.2f;
    public bool on_birst_delay = false;
    public int max_magazine_size = 3;
    public int current_magazine_size = 3;
    public bool onReload = false;


    private void Start()
    {
        m_FireButton = "Fire";
    }


    private void Update()
    {
        float CurrentTime = Time.time;

        if (on_birst_delay)
        {
            if ((CurrentTime - ShootTime) > birst_delay_time)
            {
                on_birst_delay = false;
            }
        }

        if (onReload)
        {
            if ((CurrentTime - ShootTime) > cooldown)
            {
                onReload = false;
                current_magazine_size = max_magazine_size;
            }
        }


        if (Input.GetButtonDown(m_FireButton))
        {
            Fire();
        }
    }


    private void Fire()
    {

        float CurrentTime = Time.time;
        if (onReload | on_birst_delay)
        {
            return;
        }
        current_magazine_size -= 1;
        if (current_magazine_size > 0)
        {
            on_birst_delay = true;
        }
        else
        {
            onReload = true;
        }

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
        explosion.m_MaxLifeTime = ShellLifeTime;
        explosion.owner = tankHolder;

        ShootTime = Time.time;
    }
}