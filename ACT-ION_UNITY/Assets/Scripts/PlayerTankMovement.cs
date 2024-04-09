using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class PlayerTankMovement : MonoBehaviour
{
    public float m_Speed = 12f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 300f;            // How fast the tank turns in degrees per second.
    public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
    public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.
    public int forvard_multiplyer = 1;
    public int teamNumber = 0;

    private string m_VerticalAxisName;          // The name of the input axis for moving forward and back.
    private string m_HorizontalAxisName;              // The name of the input axis for turning.
    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private float m_VerticalInputValue;         // The current value of the movement input.
    private float m_HorizontalInputValue;             // The current value of the turn input.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    private BoxCollider m_Collider;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
    }


    private void OnEnable()
    {

        // When the tank is turned on, make sure it's not kinematic.
        //m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_VerticalInputValue = 0f;
        m_HorizontalInputValue = 0f;
    }


    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        //m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        // Add tank object to InfoCollector
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.teams[teamNumber].tanks.Add(gameObject);

        m_VerticalAxisName = "Vertical";
        m_HorizontalAxisName = "Horizontal";


        GameObject cameraRig = GameObject.Find("CameraRig");
        CameraFollower follower = cameraRig.GetComponent<CameraFollower>();
        follower.m_Target = transform;

        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        // Store the value of both input axes.
        m_VerticalInputValue = Input.GetAxis(m_VerticalAxisName);
        m_HorizontalInputValue = Input.GetAxis(m_HorizontalAxisName);

        //Debug.LogWarning(m_MovementInputValue);
        EngineAudio();
    }


    private void EngineAudio()
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


    private void FixedUpdate()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move();
    }


    private void Move()
    {
        if (System.Math.Abs(m_HorizontalInputValue) < 0.05f && System.Math.Abs(m_VerticalInputValue) < 0.05f)
        {
            return;
        }

        double control_angle = (System.Math.Acos((0 * m_VerticalInputValue + 1 * m_HorizontalInputValue) /
            ((System.Math.Sqrt(m_HorizontalInputValue * m_HorizontalInputValue + m_VerticalInputValue * m_VerticalInputValue))))) * 57.3;

        double forward_angle = (System.Math.Acos((transform.forward.x * forvard_multiplyer * 1 + transform.forward.z * 0 * forvard_multiplyer) /
            ((System.Math.Sqrt(transform.forward.x * transform.forward.x + transform.forward.z * transform.forward.z))))) * 57.3;

        double delta_angle = 0;
        if (m_VerticalInputValue < 0)
        {
            control_angle = -control_angle;
        }
        if (transform.forward.z * forvard_multiplyer < 0)
        {
            forward_angle = -forward_angle;
        }
        delta_angle = control_angle - forward_angle;

        if (delta_angle < -180)
        {
            delta_angle = 360 + delta_angle;
        }
        else if (delta_angle > 180)
        {
            delta_angle = -(360 - delta_angle);
        }

        if (System.Math.Abs(delta_angle) < 3)
        {
            delta_angle = 0;
        }

        if (delta_angle > 95)
        {
            forvard_multiplyer = -1 * forvard_multiplyer;
            delta_angle = (180 - delta_angle) * forvard_multiplyer;
        }
        else if (delta_angle < -95)
        {
            forvard_multiplyer = -1 * forvard_multiplyer;
            delta_angle = (180 + delta_angle) * forvard_multiplyer;
        }

        float turn = -(float)(System.Math.Sign(delta_angle) * Time.deltaTime * m_TurnSpeed * 0.8);

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        Vector3 movement = new Vector3(0f, 0f, 0f);

        if ((m_VerticalInputValue != 0) || (m_HorizontalInputValue != 0))
        {
            movement = transform.forward * m_Speed * Time.deltaTime * 1.3f * forvard_multiplyer;
        }

        // Apply this movement to the rigidbody's position.
        Collider[] collisionArray = Physics.OverlapBox(m_Rigidbody.position + movement, m_Collider.size / 2, m_Rigidbody.rotation, ~0, QueryTriggerInteraction.Ignore);

        if (collisionArray.Length == 1)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            //Debug.Log("IM not stuck");
        }
        //m_Rigidbody.AddForce(10*movement, ForceMode.VelocityChange);
    }
}