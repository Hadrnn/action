using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static InfoCollector.Team;

public class NeuralTankShooting : TankShooting
{
    public bool onReload = false;

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
        onReload = true;

        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.shells.Add(shellInstance.GameObject());

        shellInstance.velocity = m_Velocity * m_FireTransform.forward; ;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ShellExplosion explosion = shellInstance.GetComponent<ShellExplosion>();
        explosion.m_MaxLifeTime = ShellLifeTime;
        explosion.owner = tankHolder;
        ShootTime = Time.time;
    }
}