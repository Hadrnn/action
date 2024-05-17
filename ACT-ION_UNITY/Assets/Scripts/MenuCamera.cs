using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MenuCamera : MonoBehaviour
{
    public Transform menuCamera;

    public Vector3[] positions = { new Vector3(0f, 0f, 10f),
                                   new Vector3(0f, 0f, -30f), 
                                   new Vector3(0f, 0f, 10f),
                                   new Vector3(0f, 0f, 10f) };

    public Quaternion[] rotations = {   Quaternion.Euler(0,0,0),
                                        Quaternion.Euler(0,90,0),
                                        Quaternion.Euler(0,180,0),
                                        Quaternion.Euler(0,270,0) };

    public int counter = 0;

    private Vector3 velocity;
    Quaternion rotationDerivative;

    public Vector3 targetPos;
    public Quaternion targetRotation;


    public float dampTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = positions[0];
        targetRotation = rotations[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ++counter;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            targetPos = positions[counter % 4];

        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ++counter;
            targetRotation = rotations[counter % 4];
        }

        counter = counter % 4;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        menuCamera.position = Vector3.SmoothDamp(menuCamera.position, targetPos, ref velocity, dampTime);
        menuCamera.rotation = SmoothDamp(menuCamera.rotation, targetRotation, ref rotationDerivative, dampTime);
    }

    public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;
        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }
}
