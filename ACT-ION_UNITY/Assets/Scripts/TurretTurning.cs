using UnityEngine;
using UnityEngine.UI;

public class TurretTurning: MonoBehaviour
{
    public Transform m_Transform;
    public Camera m_SomeCamera;

    private Vector3 worldPosition;
    private Plane plane = new Plane(Vector3.up, 0);

    private void FixedUpdate()
    {
        float distance;
        Ray ray = m_SomeCamera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }
        m_Transform.position = worldPosition;
    }
}