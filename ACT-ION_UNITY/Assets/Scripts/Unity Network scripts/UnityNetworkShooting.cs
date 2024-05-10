using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnityNetworkShooting : NetworkBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_Velocity = 30f;
    public float ShellLifeTime = 1f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLifeTime = 2f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

    public float cooldown = 1f;

    protected float ShootTime = 0f;
    protected string m_FireButton;                // The input axis that is used for launching shells.
    protected float m_CurrentLifeTime;         // The force that will be given to the shell when the fire button is released.
    protected float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    protected bool m_Fired;                       // Whether or not the shell has been launched with this button press.

    public InfoCollector.Team.TankHolder tankHolder { get; set; }

    private void Start()
    {
        if (!IsOwner) return;


        m_FireButton = "Fire";
    }

    private void OnEnable()
    {
        m_CurrentLifeTime = ShellLifeTime;
    }
}
