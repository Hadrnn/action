using UnityEngine;
using Unity.Netcode;

public class UnityNetworkTankMovement : NetworkBehaviour
{
    public float m_Speed = 12f;
    public float m_TurnSpeed = 300f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;
    public int forvard_multiplyer = 1;
    public SpriteRenderer m_FriendEnemy;


    public InfoCollector collector { get; private set; }


    private string m_VerticalAxisName;
    private string m_HorizontalAxisName;
    private Rigidbody m_Rigidbody;
    private float m_VerticalInputValue;
    private float m_HorizontalInputValue;
    private float m_OriginalPitch;
    private BoxCollider m_Collider;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();


        // GET SPAWN POS FROM SPAWN MANAGER
        Vector3 SpawnPos = new Vector3(10, 0, 10);
        //m_Rigidbody.MovePosition(SpawnPos);
        transform.position = SpawnPos;
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;

        m_VerticalInputValue = 0f;
        m_HorizontalInputValue = 0f;
    }


    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        GetComponent<UnityNetworkTankShooting>().tankHolder = collector.AddTank(gameObject);

        if (IsOwner)
        {
            m_VerticalAxisName = "Vertical";
            m_HorizontalAxisName = "Horizontal";

            GameSingleton.GetInstance().playerTeam = GetComponent<UnityNetworkTankShooting>().tankHolder.team.teamNumber;
            GameSingleton.GetInstance().playerClientID = OwnerClientId;

            GameObject cameraRig = GameObject.Find("CameraRig");
            CameraFollower follower = cameraRig.GetComponent<CameraFollower>();
            follower.m_Target = transform;
        }

        m_OriginalPitch = m_MovementAudio.pitch;

        collector.SetFriendEnemy();

        if(GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag)
            collector.SetBaseLights();
    }


    private void Update()
    {
        if (IsOwner)
        {
            m_VerticalInputValue = Input.GetAxis(m_VerticalAxisName);
            m_HorizontalInputValue = Input.GetAxis(m_HorizontalAxisName);

        }

        EngineAudio();
    }


    private void EngineAudio()
    {
        if (Mathf.Abs(m_VerticalInputValue) < 0.1f && Mathf.Abs(m_HorizontalInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    private void FixedUpdate()
    {
        if (!IsOwner) return;
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

        Collider[] collisionArray = Physics.OverlapBox(m_Rigidbody.position + movement, m_Collider.size / 2, m_Rigidbody.rotation, ~0, QueryTriggerInteraction.Ignore);

        if (collisionArray.Length == 1)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            //Debug.Log("IM not stuck");
        }
        //m_Rigidbody.AddForce(10*movement, ForceMode.VelocityChange);
    }

}