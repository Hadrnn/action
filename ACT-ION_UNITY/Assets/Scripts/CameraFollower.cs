using UnityEngine;

public class CameraFollower : MonoBehaviour
{
 
    public float m_DampTime = 0.1f;
    public Transform m_Target;
    
    private Camera m_Camera;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;

    void Start()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        if (m_Target)
        {
            m_DesiredPosition = m_Target.position;
            Vector3 newPosition = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);

            transform.position = newPosition;
        }

    }
}
