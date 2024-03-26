using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotTurretTurning : MonoBehaviour
{
    private BotTankShooting Gun;
    private int teamNumber;
    private int delay = 150;
    private int delay_counter = 0;

    private void Start()
    {
        Gun = gameObject.GetComponentInParent<BotTankShooting>();
        teamNumber = gameObject.GetComponentInParent<BotTankMovement>().teamNumber;
    }
    void Update()
    {
        // Распознавание танка игрока и бота по индексу в массиве - костыль, нужно переделать
        Vector3 TargetPos;
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        TargetPos = collector.teams[teamNumber - 1].tanks[0].transform.position; 
        transform.LookAt(TargetPos, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        Vector3 BotPos = collector.teams[teamNumber].tanks[0].transform.position;

        Vector3 direction = (TargetPos - BotPos).normalized;
        float distance = Vector3.Distance(BotPos, TargetPos);
        Collider PlayerCollider = collector.teams[teamNumber - 1].tanks[0].GetComponent<Collider>();
        RaycastHit hit;
        Physics.Raycast(BotPos, direction, out hit, distance);
        
        if (hit.collider == PlayerCollider)
        {
 /*           Debug.Log("Hochu strelyatb");*/
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
            if (hit.collider == collector.teams[teamNumber].tanks[0].GetComponent<Collider>())
            {
                Debug.Log("huy");
            }
        }
        
    }
}
