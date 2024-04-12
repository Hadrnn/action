using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class PlayerMovement : TankMovement
{
    protected string m_VerticalAxisName;          // The name of the input axis for moving forward and back.
    protected string m_HorizontalAxisName;              // The name of the input axis for turning.

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


    private void FixedUpdate()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move();
    }

    protected abstract void Move();

}
