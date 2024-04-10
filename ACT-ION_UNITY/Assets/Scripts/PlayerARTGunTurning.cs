using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerARTGunTurning : MonoBehaviour
{
    public Camera m_Camera;
    private Vector3 mousePos;
    private Plane plane = new Plane(Vector3.up, 0);
    public float shel_speed = 25;
    private Vector3 prew_angles = new Vector3(0, 45f, 0);
    // Start is called before the first frame update
    void Start()
    {
        GameObject cameraRig = GameObject.Find("CameraRig");
        m_Camera = cameraRig.GetComponentInChildren<Camera>();
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
        Vector3 MyPos = gameObject.GetComponentInParent<Transform>().position;
        
        Vector3 DeltaPos = MyPos - mousePos;
        float dist = DeltaPos.magnitude;
        angles.y = Mathf.Asin(9.81f*dist/(shel_speed*shel_speed))*100;

        transform.Rotate(angles - prew_angles);
        prew_angles = angles;
    }
}
