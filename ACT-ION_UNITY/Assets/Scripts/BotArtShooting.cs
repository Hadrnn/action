using UnityEngine;
using Unity.VisualScripting;

public class BotArtShooting : BotShooting
{
    public BotArtTurretTurning turret;
    public float start_angle;
    public float g;
    public float shell_speed;

    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        m_CurrentLifeTime = m_MinLifeTime;

        //m_AimSlider.value = m_MinLifeTime;
    }


    private void Update()
    {
        float CurrentTime = Time.time;
        if ((CurrentTime - ShootTime) > cooldown)
        {
            onReload = false;
        }
    }
    public override void Fire()
    {

        float CurrentTime = Time.time;
        if ((CurrentTime - ShootTime) < cooldown)
        {
            return;
        }

        m_Fired = true;
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

        BotArtShellExplosion explosion = shellInstance.GetComponent<BotArtShellExplosion>();
/*        Debug.Log(explosion.forward);*/
        explosion.forward = turret.transform.forward;
        explosion.owner = tankHolder;

        explosion.start_angle = start_angle;
        explosion.m_MaxLifeTime = m_CurrentLifeTime;
        explosion.tank = this;
        explosion.g = g;
        explosion.velocity = shell_speed;
        m_CurrentLifeTime = m_MinLifeTime;
        ShootTime = Time.time;
    }
}