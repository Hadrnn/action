using Unity.Netcode;
using UnityEngine;

public class UnityNetworkShooting : NetworkBehaviour
{
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public AudioSource m_ShootingAudio;
    public AudioClip m_FireClip;
    public float m_Velocity = 30f;
    public float ShellLifeTime = 1f;

    public float cooldown = 1f;

    protected float ShootTime = 0f;
    protected string m_FireButton;

    public InfoCollector.Team.TankHolder tankHolder { get; set; }

    private void Start()
    {
        if (!IsOwner) return;


        m_FireButton = "Fire";
    }
}
