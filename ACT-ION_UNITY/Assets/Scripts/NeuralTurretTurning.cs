using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralTurretTurning : MonoBehaviour
{
    private NeuralTankShooting Gun;
    private int teamNumber;
    private void Start()
    {
        Gun = gameObject.GetComponentInParent<NeuralTankShooting>();
        teamNumber = gameObject.GetComponentInParent<NeuralTankMovement>().teamNumber;

    }
    void Update()
    {
        // Распознавание танка игрока и бота по индексу в массиве - костыль, нужно переделать
        Vector3 TargetPos;
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        TargetPos = collector.teams[teamNumber].tanks[1].transform.position;
        transform.LookAt(TargetPos, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        Vector3 BotPos = collector.teams[teamNumber].tanks[0].transform.position;

        Vector3 direction = (TargetPos - BotPos).normalized;
        float distance = Vector3.Distance(BotPos, TargetPos);
        Collider PlayerCollider = collector.teams[teamNumber].tanks[1].GetComponent<Collider>();
        RaycastHit hit;
        if (Physics.Raycast(BotPos, direction, out hit, distance))
        {
            if (hit.collider == PlayerCollider)
            {
                Gun.Fire();
            }
        }
    }
}
