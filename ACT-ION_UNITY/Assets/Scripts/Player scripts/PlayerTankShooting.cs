using UnityEngine;
using Unity.VisualScripting;

public class PlayerTankShooting : TankShooting
{
    private string m_FireButton;


    private void Start()
    {
         m_FireButton = "Fire";
    }


    private void Update()
    {
        if (GameSingleton.GetInstance().paused) return;

        if (Input.GetButton(m_FireButton))
        {
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
    }
}