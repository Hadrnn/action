using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerARTGunTurning : MonoBehaviour
{
    public Camera m_Camera;
    public Transform FireTransform;
    public bool direct = true;

    private Vector3 mousePos;
    private Plane plane = new Plane(Vector3.up, 0);
    private float shel_speed = 40;
    private Vector3 prew_angles = new Vector3(0, 45f, 0);
    private float g = 20;
    protected PlayerArtShooting Gun;
    // Start is called before the first frame update
    void Start()
    {
        GameObject cameraRig = GameObject.Find("CameraRig");
        m_Camera = cameraRig.GetComponentInChildren<Camera>();
        Gun = gameObject.GetComponentInParent<PlayerArtShooting>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 angles = new Vector3(0,0,0);
        float distance;
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out distance))
        {
            mousePos = ray.GetPoint(distance);
        }
        if (direct)
        {
            Vector3 MyPos = FireTransform.position;
            Vector3 DeltaPos = MyPos - mousePos;
            float dist = DeltaPos.magnitude + 1;
            float max_dist = shel_speed * shel_speed / g;
            if (dist > max_dist)
            {
                dist = max_dist;
            }
            angles.y = 90 - Mathf.Rad2Deg * 0.5f * Mathf.Asin(g * dist / (shel_speed * shel_speed));
            Gun.start_angle = 90 - angles.y;
            transform.Rotate(angles - prew_angles);
            prew_angles = angles;
        }
        else
        {
            Vector3 MyPos = FireTransform.position;
            Vector3 DeltaPos = MyPos - mousePos;
            float dist = DeltaPos.magnitude + 1;
            float max_dist = shel_speed * shel_speed / g;
            if (dist > max_dist)
            {
                dist = max_dist;
            }
            angles.y = Mathf.Rad2Deg * 0.5f * Mathf.Asin(g * dist / (shel_speed * shel_speed));
            Gun.start_angle = 90 - angles.y;
            transform.Rotate(angles - prew_angles);
            prew_angles = angles;
        }
    }
}
