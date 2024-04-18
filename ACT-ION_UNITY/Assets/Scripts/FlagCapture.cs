using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCapture : MonoBehaviour
{
    public int teamNumber = 0;
    public Transform teamBase;

    private bool IsCaptured;

    private void Start()
    {
        if(teamBase.GetComponent<FlagBase>().teamNumber != teamNumber)
        {
            throw new System.Exception("NOT MATHCING TEAM NUMBER OF FLAG AND FLAG BASE");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FlagBase PossiblyBase = other.GetComponent<FlagBase>();
        if (PossiblyBase && PossiblyBase.teamNumber != teamNumber)
        {
            Debug.Log("Im on Enemy Base");
            transform.SetParent(null);
            transform.position = teamBase.position;
        }
        
        TankMovement tank = other.GetComponent<TankMovement>();

        if (!tank || IsCaptured) return;


        if (tank.teamNumber == teamNumber && transform.parent == null)
        {
            Debug.Log("Im returning to base");
            
            transform.position = teamBase.position;
            return;
        }

        transform.SetParent(other.transform);

        transform.position = other.transform.position;
    }


}
