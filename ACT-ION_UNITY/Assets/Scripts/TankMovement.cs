using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TankMovement : MonoBehaviour
{
    public float m_Speed = 12f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 300f;            // How fast the tank turns in degrees per second.
    public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
    public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.
    public int teamNumber = 1;

    protected Rigidbody m_Rigidbody;              // Reference used to move the tank.
    protected float m_VerticalInputValue = 0;         // The current value of the movement input.
    protected float m_HorizontalInputValue = 0;             // The current value of the turn input.
    protected float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    protected BoxCollider m_Collider;
    protected int forvard_multiplyer = 1;

    protected void EngineAudio()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs(m_VerticalInputValue) < 0.1f && Mathf.Abs(m_HorizontalInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // ... change the clip to idling and play it.
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                // ... change the clip to driving and play.
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    public static Transform FindClosestEnemy(int teamNumber, Transform tank, InfoCollector collector)
    {
        Transform Target = tank;
        float closestDistance = 100000;
        for (ushort i = 0; i < collector.teams.Count; ++i)
        {
            if (i == teamNumber) continue;

            for (ushort j = 0; j < collector.teams[i].tanks.Count; ++j)
            {
                float currentDistance = Vector3.Distance(tank.position, collector.teams[i].tanks[j].transform.position);
                if (currentDistance < closestDistance)
                {
                    Target = collector.teams[i].tanks[j].transform;
                }
            }
        }
        return Target;

    }
}
