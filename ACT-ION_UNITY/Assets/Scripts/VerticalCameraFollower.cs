using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalCameraFollower : MonoBehaviour
{
    // Start is called before the first frame update
    public float m_DampTime = 0.1f;
    public Transform m_Target;
    public float m_CameraHeight = 10.5f;
    
    
    private Camera m_Camera;
    private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 m_DesiredPosition;

    void Start()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        m_DesiredPosition = m_Target.position;
        Vector3 newPosition = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);

        //Vector3 newPosition = m_Target.position;
        //newPosition.y = m_CameraHeight;
        transform.position = newPosition;
    }
}
