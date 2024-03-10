using UnityEngine;
using UnityEngine.UI;

public class TurretTurning: MonoBehaviour
{
    public Transform m_Transform;

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 direction_3d = mousePosition - m_Transform.position;
        Vector2 direction = new Vector2(direction_3d[0], direction_3d[1]);
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        m_Transform.eulerAngles = new Vector3(0, angle, 0);
        Debug.LogWarning(angle);
    }
}