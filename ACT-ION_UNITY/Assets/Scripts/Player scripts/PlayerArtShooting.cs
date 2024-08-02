using UnityEngine;
using Unity.VisualScripting;

public class PlayerArtShooting : TankShooting, IArtShooting
{
    public PlayerTurretTurning turret;
    public float start_angle;
    public float g;
    public float shell_speed;

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

        Vector3 tank_pos = transform.position;
        Vector3 forvard = tank_pos - m_FireTransform.position;
        forvard = forvard.normalized;
        Vector3 add_comp = new Vector3(0, 0, 0);
        add_comp.x = forvard.x;
        add_comp.z = forvard.z;

        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position - add_comp, m_FireTransform.rotation) as Rigidbody;

        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.shells.Add(shellInstance.GameObject());

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ArtShellExplosion explosion = shellInstance.GetComponent<ArtShellExplosion>();
        explosion.forward = turret.transform.forward;
        explosion.owner = tankHolder;
        explosion.start_angle = start_angle;
        explosion.m_MaxLifeTime = ShellLifeTime;
        explosion.tank = this;
        explosion.g = g;
        explosion.velocity = shell_speed;
        ShootTime = Time.time;
    }
}