using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : MonoBehaviour
{

    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_Velocity = 30f;
    public float ShellLifeTime = 1f;        // The force given to the shell if the fire button is not held.
    public float cooldown = 1f;

    public InfoCollector.Team.TankHolder tankHolder { get; set; }
    protected float ShootTime = 0f;
}
