using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class PlayerMovement : TankMovement
{
    protected string m_VerticalAxisName;          // The name of the input axis for moving forward and back.
    protected string m_HorizontalAxisName;              // The name of the input axis for turning.

    private void OnEnable()
    {

        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_VerticalInputValue = 0f;
        m_HorizontalInputValue = 0f;
    }


    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        // Add tank object to InfoCollector
        if (!collector) collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        //else Debug.Log("Collector already set");

        GetComponent<TankShooting>().tankHolder = collector.AddTank(gameObject);

        GameSingleton.GetInstance().playerTeam = teamNumber;

        // Add tank object to InfoCollector
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
        if (GameSingleton.GetInstance().paused) return;

        Move();
    }

    protected void Move()
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

        float turn = -(float)(System.Math.Sign(delta_angle) * Time.deltaTime * m_TurnSpeed);

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        Vector3 movement = new Vector3(0f, 0f, 0f);

        if ((m_VerticalInputValue != 0) || (m_HorizontalInputValue != 0))
        {
            movement = transform.forward * m_Speed * Time.deltaTime * forvard_multiplyer;
        }

        // Apply this movement to the rigidbody's position.
        Collider[] collisionArray = Physics.OverlapBox(m_Rigidbody.position + movement, m_Collider.size / 2, m_Rigidbody.rotation, ~0, QueryTriggerInteraction.Ignore);

        if (collisionArray.Length == 1)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            //Debug.Log("IM not stuck");
        }
        /*        Debug.Log(teamNumber);*/
        //m_Rigidbody.AddForce(10*movement, ForceMode.VelocityChange);
    }

}
