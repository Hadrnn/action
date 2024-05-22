using Unity.Netcode;
using UnityEngine;

public class UnityNetworkAPCShooting : UnityNetworkShooting
{
    public float birst_delay_time = 0.2f;
    public int max_magazine_size = 3;

    private int current_magazine_size = 3;
    private bool onReload = false;
    private bool on_birst_delay = false;


    private void Update()
    {

        if (!IsOwner) return;

        if (Input.GetButtonDown(m_FireButton))
        {
            Fire();
        }

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


    private void Fire()
    {
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

        NetworkObject new_shell = shellInstance.GetComponent<NetworkObject>();

        new_shell.Spawn();
        //new_shell.RemoveOwnership();

        // Set the shell's velocity to the launch force in the fire position's forward direction.


        shellInstance.velocity = m_Velocity * m_FireTransform.forward;
    }
}
