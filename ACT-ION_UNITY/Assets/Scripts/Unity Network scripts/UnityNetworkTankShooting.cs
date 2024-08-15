using UnityEngine;
using Unity.Netcode;

public class UnityNetworkTankShooting : UnityNetworkShooting
{

    private void Update()
    {

        if (!IsOwner) return;

        if (Input.GetButton(m_FireButton))
        {
            Fire();
        }
    }


    private void Fire()
    {
        // Not to shoot while physics is stopped
        if (Time.timeScale == 0) return;

        float CurrentTime = Time.time;
        if ((CurrentTime - ShootTime) < cooldown)
        {
            return;
        }

        FireServerRpc();

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ShootTime = Time.time;
    }

    [ServerRpc]
    public void FireServerRpc()
    {

        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        UnityNetworkShellExplosion explosion = shellInstance.GetComponent<UnityNetworkShellExplosion>();
        explosion.owner = tankHolder;
        explosion.m_MaxLifeTime = ShellLifeTime;

        NetworkObject new_shell = shellInstance.GetComponent<NetworkObject>();

        new_shell.Spawn();
        //new_shell.RemoveOwnership();


        shellInstance.velocity = m_Velocity * m_FireTransform.forward;
    }

}