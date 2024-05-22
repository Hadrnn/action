using UnityEngine;
using Unity.Netcode;

public class UnityNetworkTankShooting : UnityNetworkShooting
{

    private void Update()
    {

        if (!IsOwner) return;

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

        FireServerRpc();

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        ShootTime = Time.time;
    }

    [ServerRpc]
    public void FireServerRpc()
    {        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        UnityNetworkShellExplosion explosion = shellInstance.GetComponent<UnityNetworkShellExplosion>();
        explosion.owner = tankHolder;
        explosion.m_MaxLifeTime = ShellLifeTime;

        NetworkObject new_shell = shellInstance.GetComponent<NetworkObject>();

        new_shell.Spawn();
        //new_shell.RemoveOwnership();

        // Set the shell's velocity to the launch force in the fire position's forward direction.


        shellInstance.velocity = m_Velocity * m_FireTransform.forward;
    }

}