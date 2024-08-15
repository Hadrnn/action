using System.Collections.Generic;
using UnityEngine;

public class BotAPCTurretTurning : MonoBehaviour 
{
    protected BotAPCShooting Gun;
    public float delay = 1;
    protected float delay_counter = 0;
    protected InfoCollector collector;
    protected Transform FireTransform;
    protected Transform BotTransform;

    public float prew_t;
    public float prew_m;
    public float prew_alpha;
    private Queue<Vector3> prew_target_poses = new Queue<Vector3>();

    private void Start()
    {
        Gun = gameObject.GetComponentInParent<BotAPCShooting>();

        collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        FireTransform = GetComponentInChildren<Transform>().Find("FireTransform");

        BotTransform = gameObject.GetComponentInParent<TankMovement>().gameObject.transform;

        Vector3 prew_target_pos = TankMovement.FindClosestEnemy(BotTransform.GetComponent<TankMovement>().teamNumber, BotTransform, collector).position;
        for (int i = 0; i < 40; i++)
        {
            prew_target_poses.Enqueue(prew_target_pos);
        }
    }
    void FixedUpdate()
    {
        Transform Target = TankMovement.FindClosestEnemy(BotTransform.GetComponent<TankMovement>().teamNumber, BotTransform, collector);

        if (Target == BotTransform)
        {
            return;
        }

        Vector3 BotPos = BotTransform.position;
        Vector3 TargetPos = Target.position;
        if ((TargetPos - prew_target_poses.Peek()).magnitude > 1.2)
        {
            float l = TargetPos.x - BotPos.x;
            float h = TargetPos.z - BotPos.z;
            float v1 = Gun.m_Velocity;
            float v2x = Target.GetComponent<TankMovement>().m_Speed * Target.forward.x * Target.GetComponent<TankMovement>().forvard_multiplyer;
            float v2z = Target.GetComponent<TankMovement>().m_Speed * Target.forward.z * Target.GetComponent<TankMovement>().forvard_multiplyer;
            float k = l / h;
            float m = (v2z * k - v2x) / v1;
            float t1 = (2 * k - Mathf.Sqrt(4 * k * k - 4 * (m * m - 1))) / (2 * (m - 1));
            float t2 = (2 * k + Mathf.Sqrt(4 * k * k - 4 * (m * m - 1))) / (2 * (m - 1));
            float t;
            if (l > 0)
            {
                if (Mathf.Abs(t2) > Mathf.Abs(t1))
                {
                    t = t1;
                }
                else
                {
                    t = t2;
                }
            }
            else
            {
                if (Mathf.Abs(t2) < Mathf.Abs(t1))
                {
                    t = t1;
                }
                else
                {
                    t = t2;
                }
            }
            float alpha = 2 * Mathf.Atan(t);
            Vector3 gun_length = (FireTransform.position - BotPos);
            gun_length.y = 0;
            float dL = v1 * Mathf.Cos(alpha) * l / (v1 * Mathf.Cos(alpha) - v2x) + gun_length.magnitude * Mathf.Cos(alpha) * transform.forward.x / Mathf.Abs(transform.forward.x);
            float dH = v1 * Mathf.Sin(alpha) * h / (v1 * Mathf.Sin(alpha) - v2z) + gun_length.magnitude * Mathf.Sin(alpha) * transform.forward.x / Mathf.Abs(transform.forward.x);
            prew_target_poses.Dequeue();
            prew_target_poses.Enqueue(TargetPos);
            TargetPos.x = BotPos.x + dL;
            TargetPos.z = BotPos.z + dH;
        }
        else
        {
            prew_target_poses.Dequeue();
            prew_target_poses.Enqueue(TargetPos);
        }




        transform.LookAt(TargetPos, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        if ((BotPos - TargetPos).magnitude < 20)
        {
            Vector3 GunPos = FireTransform.position;
            float distance = Vector3.Distance(GunPos, TargetPos);
            Collider PlayerCollider = Target.GetComponent<Collider>();
            Vector3 shooting_square = new Vector3(3, 0.1f, distance);
            Collider[] collisionArray = Physics.OverlapBox(GunPos + (TargetPos - GunPos) / 2, shooting_square / 2, FireTransform.rotation, ~0, QueryTriggerInteraction.Ignore);

            if (collisionArray.Length == 0)
            {
                Gun.Fire();
                if (Gun.current_magazine_size == 0)
                {
                    delay_counter = Time.time;
                }
            }
            else if (collisionArray.Length == 1 & collisionArray[0] == PlayerCollider)
            {
                Gun.Fire();
                if (Gun.current_magazine_size == 0)
                {
                    delay_counter = Time.time;
                }
            }
        }
    }
}