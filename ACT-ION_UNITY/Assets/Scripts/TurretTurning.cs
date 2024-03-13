using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TurretTurning: NetworkBehaviour
{
    public Transform m_Tank;
    public Transform m_Turret;

    private Camera m_Camera;
    private Vector3 mousePosition = new Vector3(0,0,0);
    private Plane plane = new Plane(Vector3.up, 0);

    private void Start()
    {
        m_Camera = Camera.main;
        //Debug.LogWarning("Started Turret turn");
    }

    private void FixedUpdate()
    {
        float distance;
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        //Debug.LogWarning("Turning turret");

        if (plane.Raycast(ray, out distance))
        {
            mousePosition = ray.GetPoint(distance);
        }

        //Vector3 turretToMouse = m_Tank.position - mousePosition;
        ////m_Transform.position = worldPosition;
        //turretToMouse.y = 0;

        //float angle = Vector3.Angle(Vector3.forward,turretToMouse);
        //Debug.LogWarning(string.Format("{0:N2}", angle));
        //Vector3 toTurn = new Vector3(0, angle, 0);
        ////Debug.LogWarning(toTurn);
        //transform.Rotate(toTurn);
        m_Turret.LookAt(mousePosition, Vector3.up);
    }
}