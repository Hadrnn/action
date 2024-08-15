using UnityEngine;

public class TankMovement : MonoBehaviour, ITankMovement
{
    public const int teamNotSet = -1;

    public float m_Speed = 12f;
    public float m_TurnSpeed = 300f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;

    public SpriteRenderer m_FriendEnemy;
    public int forvard_multiplyer = 1;

    public string playerName = "";

    public int teamNumber { get; set; }
    public static InfoCollector collector { get; protected set; }

    protected Rigidbody m_Rigidbody;              // Reference used to move the tank.
    protected float m_VerticalInputValue = 0;         // The current value of the movement input.
    protected float m_HorizontalInputValue = 0;             // The current value of the turn input.
    protected float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    protected BoxCollider m_Collider;

    private float m_PitchRange = 0.1f;           // The amount by which the pitch of the engine noises can vary.

    private void Awake()
    {
        teamNumber = teamNotSet;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
    }

    public void setColor(Color colorToSet)
    {
        m_FriendEnemy.color = colorToSet;
    }
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

    public static Transform FindClosestEnemy(int teamNumber_, Transform tank, InfoCollector collector)
    {

        Transform Target = tank;
        float closestDistance = 100000;
        for (ushort i = 0; i < collector.teams.Count; ++i)
        {
            if (collector.teams[i].teamNumber == teamNumber_) continue;

            for (ushort j = 0; j < collector.teams[i].tanks.Count; ++j)
            {
                Transform Enemy = collector.teams[i].tanks[j].tank.transform;
                if (!Enemy.gameObject.activeSelf) continue;

                float currentDistance = Vector3.Distance(tank.position, Enemy.position);
                if (currentDistance < closestDistance)
                {
                    Target = Enemy;
                    closestDistance = currentDistance;
                }
            }
        }
        return Target;
    }
}
