using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0f, 0f, 0f);

    public Vector3[] positions = { new Vector3(0f, 0f, 0f),
                                   new Vector3(0f, 0f, 0f), 
                                   new Vector3(0f, 0f, 0f),
                                   new Vector3(0f, 0f, 0f) };

    public Quaternion[] rotations = {   Quaternion.Euler(0,0,0),
                                        Quaternion.Euler(0,90,0),
                                        Quaternion.Euler(0,180,0),
                                        Quaternion.Euler(0,270,0) };

    public Vector3[] offsets = { new Vector3(-1f, 0f, 0f),
                                   new Vector3(0f, 0f, -1f),
                                   new Vector3(1f, 0f, 0f),
                                   new Vector3(0f, 0f, 1f) };

    public int counter = 0;

    private Vector3 velocity;
    Quaternion rotationDerivative;

    public Vector3 targetPos;
    public Quaternion targetRotation;


    public float dampTime = 1f;
    public bool OnMainMenu = true;

    private void Awake()
    {
        int startTank = GameSingleton.GetInstance().currentTank;
        transform.position = positions[startTank] + offsets[startTank];
        transform.rotation = rotations[startTank];
    }

    // Start is called before the first frame update
    void Start()
    {
        //int startTank = Random.Range(0, 4);
        //GameSingleton.GetInstance().currentTank = startTank;
        //targetPos = positions[startTank];
        //targetRotation = rotations[startTank];
    }

    private void Update()
    {
        if (OnMainMenu)
        {
            targetPos = positions[GameSingleton.GetInstance().currentTank] + offsets[GameSingleton.GetInstance().currentTank];
        }
        else
        {
            targetPos = positions[GameSingleton.GetInstance().currentTank];
            targetRotation = rotations[GameSingleton.GetInstance().currentTank];
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, dampTime);
        transform.rotation = SmoothDamp(transform.rotation, targetRotation, ref rotationDerivative, dampTime);
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

    public void setMainMenu(bool toSet)
    {
        OnMainMenu = toSet;
    }
}
