using UnityEngine;

public class UnityNetworkARTShellExplosion : UnityNetworkShellExplosion
{
    public Vector3 forward;
    public float start_angle;
    public IArtShooting tank;
    public float velocity = 40f;
    public float g = 20f;

    private Rigidbody m_Rigidbody;
    private float up_speed;

    private void Start()
    {
        startTime = Time.time;
        m_Rigidbody = GetComponent<Rigidbody>();
        forward *= Mathf.Cos(Mathf.Deg2Rad * start_angle);
        forward.y = Mathf.Sin(Mathf.Deg2Rad * start_angle);
        up_speed = velocity * Mathf.Sin(Mathf.Deg2Rad * start_angle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.GetComponent<UnityNetworkFlagCapture>() || other.GetComponent<UnityNetworkFlagBase>()) return;

        if (other.gameObject.GetComponent<IArtShooting>() != null)
        {
            if (other.gameObject.GetComponent<IArtShooting>() == tank)
            {

                return;
            }
        }
        DieClientRpc();
        Explode();
    }


    private void FixedUpdate()
    {
        if (m_Rigidbody.position.y < 0.5)
        {
            if (IsServer)
            {
                DieClientRpc();
                Explode();
            }
        }
        Vector3 movement = Vector3.zero;
        up_speed = up_speed - g * Time.deltaTime;
        movement.x = velocity * Time.deltaTime * forward.x;
        movement.z = velocity * Time.deltaTime * forward.z;
        movement.y = up_speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        Vector3 speed_forward = movement.normalized;
        Vector3 speed_forward_another_axis = new Vector3(speed_forward.y, 0, Mathf.Sqrt(speed_forward.x * speed_forward.x + speed_forward.z * speed_forward.z));
        Vector3 forward_another_axis = new Vector3(transform.forward.y, 0, Mathf.Sqrt(transform.forward.x * transform.forward.x + transform.forward.z * transform.forward.z));
        Vector3 Axis = new Vector3(0, 1, 0);
        float turn = -Vector3.SignedAngle(forward_another_axis, speed_forward_another_axis, Axis);
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
