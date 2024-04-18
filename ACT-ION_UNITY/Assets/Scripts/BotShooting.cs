using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotShooting : TankShooting
{
    public bool onReload = false;


    protected BotMovement Body;
    // How fast the launch force increases, based on the max charge time.

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentLifeTime = m_MinLifeTime;
        Body = GetComponent<BotMovement>();
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

    public virtual void Fire()
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
        explosion.owner = tankHolder;

        m_CurrentLifeTime = m_MinLifeTime;
        ShootTime = Time.time;

        Body.counter = BotMovement.discret;
    }
}
