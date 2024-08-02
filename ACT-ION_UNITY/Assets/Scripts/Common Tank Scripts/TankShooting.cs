using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : MonoBehaviour
{

    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public AudioSource m_ShootingAudio;
    public AudioClip m_FireClip;
    public float m_Velocity = 30f;
    public float ShellLifeTime = 1f;
    public float cooldown = 1f;

    public InfoCollector.Team.TankHolder tankHolder { get; set; }
    protected float ShootTime = 0f;
}
