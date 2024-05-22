using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;

public class BotArtShooting : BotShooting, ArtShooting
{
    public BotArtTurretTurning turret;
    public float start_angle;
    public float g;
    public float shell_speed;


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

        //m_Fired = true;
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
/*        Debug.Log(explosion.forward);*/
        explosion.forward = turret.transform.forward;
        explosion.owner = tankHolder;

        explosion.start_angle = start_angle;
        explosion.m_MaxLifeTime = ShellLifeTime;
        explosion.tank = this;
        explosion.g = g;
        explosion.velocity = shell_speed;



        //GameObject cameraRig = GameObject.Find("CameraRig");
        //CameraFollower follower = cameraRig.GetComponent<CameraFollower>();
        //if (!follower.m_Target)
        //{
        //    follower.m_Target = shellInstance.transform;
        //}



        ShootTime = Time.time;
    }
}