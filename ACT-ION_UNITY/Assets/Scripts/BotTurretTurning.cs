using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotTurretTurning : MonoBehaviour
{
    protected BotShooting Gun;
    protected int teamNumber;
    protected int delay = 150;
    protected int delay_counter = 0;

    protected InfoCollector collector;
    protected Transform FireTransform;
    protected Transform BotTransform;

    private void Start()
    {
        Gun = gameObject.GetComponentInParent<BotShooting>();

        teamNumber = gameObject.GetComponentInParent<TankMovement>().teamNumber;

        collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        FireTransform = GetComponentInChildren<Transform>().Find("FireTransform");

        BotTransform = gameObject.GetComponentInParent<TankMovement>().gameObject.transform;

    }
    void Update()
    {
        Transform Target = TankMovement.FindClosestEnemy(teamNumber,BotTransform,collector);

        if (Target.Equals(BotTransform)) return;

        Vector3 BotPos = BotTransform.position;
        Vector3 TargetPos = Target.position;
        transform.LookAt(TargetPos, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);


        Vector3 GunPos = FireTransform.position;

        Vector3 direction = (TargetPos - BotPos).normalized;
        float distance = Vector3.Distance(BotPos, TargetPos);
        Collider PlayerCollider = Target.GetComponent<Collider>();
        RaycastHit hit;
        Physics.Raycast(GunPos, direction, out hit, distance * 2);
        
        if (hit.collider == PlayerCollider)
        {
            if (delay_counter < delay)
            {
                delay_counter += 1;
            }
            else
            {
                Gun.Fire();
                delay_counter = 0;
            }
        }
        else
        {
            if (hit.collider == BotTransform.GetComponent<Collider>())
            {
                Debug.Log("Bot hit his collider with raycast");
            }
        }
    }
}
