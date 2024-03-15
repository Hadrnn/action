using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UnityNetworkTurretTurning : NetworkBehaviour {
    public Camera m_Camera;
    private Vector3 mousePos;
    private Plane plane = new Plane(Vector3.up, 0);

    private void Start()
    {
        GameObject cameraRig = GameObject.Find("CameraRig");
        m_Camera = cameraRig.GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        float distance;
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out distance))
        {
            mousePos = ray.GetPoint(distance);
        }

        transform.LookAt(mousePos, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}