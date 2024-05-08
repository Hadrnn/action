using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotAPCShooting : BotShooting
{
    public float birst_delay_time = 0.2f;
    public bool on_birst_delay = false;
    public int max_magazine_size = 3;
    public int current_magazine_size = 3;
    void Update()
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
    }

    public override void Fire()
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
        
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        collector.shells.Add(shellInstance.GameObject());

        shellInstance.velocity = m_Velocity * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ShellExplosion explosion = shellInstance.GetComponent<ShellExplosion>();
        explosion.m_MaxLifeTime = ShellLifeTime;
        explosion.owner = tankHolder;

        ShootTime = Time.time;

        Body.counter = Body.discret;
    }
}
