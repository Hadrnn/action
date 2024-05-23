using System.Collections.Generic;
using UnityEngine;

public class BotARTGunTurning : MonoBehaviour
{
    public Transform FireTransform;
    public bool direct = true;
    public float g;
    public float shel_speed;
    public BotArtTurretTurning turret;

    private Vector3 prew_angles = new Vector3(0, 45f, 0);
    private Queue<Vector3> prew_target_poses = new Queue<Vector3>();
    protected InfoCollector collector;
    protected BotArtShooting Gun;
    protected Transform BotTransform;

    void Start()
    {
        BotTransform = gameObject.GetComponentInParent<TankMovement>().gameObject.transform;
        Gun = gameObject.GetComponentInParent<BotArtShooting>();
        collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        Vector3 prew_target_pos = TankMovement.FindClosestEnemy(BotTransform.GetComponent<TankMovement>().teamNumber, BotTransform, collector).position;
        for (int i = 0; i < 10; i++)
        {
            prew_target_poses.Enqueue(prew_target_pos);
        }
    }

    void FixedUpdate()
    {
        if (GameSingleton.GetInstance().paused) return;

        Vector3 angles = new Vector3(0, 0, 0);
        Transform Target = TankMovement.FindClosestEnemy(BotTransform.GetComponent<TankMovement>().teamNumber, BotTransform, collector);
        if (Target.Equals(BotTransform))
        {
            return;
        }

        Vector3 TargetPos = Target.position;
        Collider PlayerCollider = Target.GetComponent<Collider>();
        Vector3 MyPos = FireTransform.position;
        Vector3 direction = (TargetPos - MyPos).normalized;
        Vector3 DeltaPos = MyPos - TargetPos;
        float dist = DeltaPos.magnitude + 1;
        RaycastHit hit;
        Physics.Raycast(MyPos, direction, out hit, dist * 2);
        float V;
        float G;
        float mult;
        if (hit.collider == PlayerCollider)
        {
            direct = true;
            V = shel_speed;
            G = g;
            mult = 1;
        }
        else
        {
            direct = false;
            V = shel_speed * 2;
            G = g * 8;
            mult = 1f;
        }

        if ((TargetPos - prew_target_poses.Peek()).magnitude > 2)
        {
            Vector3 Target_forward = Target.forward;
            int Target_forward_mult = Target.GetComponent<TankMovement>().forvard_multiplyer;
            float Target_speed = Target.GetComponent<TankMovement>().m_Speed;
            float median_time;

            median_time = 2f * V * Mathf.Sin((90 - prew_angles.y) * Mathf.Deg2Rad) / G;
            if(!direct)
            {
                median_time *= mult;
            }
            Vector3 predict_target_pos = TargetPos + Target_forward_mult * Target_speed * Target_forward * median_time;
            prew_target_poses.Dequeue();
            prew_target_poses.Enqueue(TargetPos);
            TargetPos = predict_target_pos;
            DeltaPos = MyPos - TargetPos;
            dist = DeltaPos.magnitude;
        }
        else
        {
            prew_target_poses.Dequeue();
            prew_target_poses.Enqueue(TargetPos);
        }

        turret.transform.LookAt(TargetPos, Vector3.up);
        float max_dist = V * V / G;
        bool can_shoot = true;
        if (dist > max_dist)
        {
            can_shoot = false;
            dist = max_dist;
        }
        angles.y = Mathf.Rad2Deg * 0.5f * Mathf.Asin(G * dist / (V * V));
        if (direct)
        {
            angles.y = 90 - angles.y;
        }
        Gun.start_angle = 90 - angles.y;
        Gun.g = G;
        Gun.shell_speed = V;
        transform.Rotate(angles - prew_angles);
        prew_angles = angles;
        if (can_shoot)
        {
            Gun.Fire();
        }
    }
}
