using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnityNetworkFlagCapture : NetworkBehaviour
{
    public NetworkVariable<int> teamNumber;
    public Transform teamBase;

    private bool IsCaptured;

    private void Start()
    {
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");

        foreach (GameObject CTFBase in bases)
        {
            UnityNetworkFlagBase currBase = CTFBase.GetComponent<UnityNetworkFlagBase>();

            if (currBase.teamNumber.Value == teamNumber.Value)
            {
                Debug.Log("Flag found a base");
                teamBase = currBase.transform;
                break;
            }

        }


        if (teamBase.GetComponent<UnityNetworkFlagBase>().teamNumber.Value != teamNumber.Value)
        {
            throw new System.Exception("NOT MATHCING TEAM NUMBER OF FLAG AND FLAG BASE");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        UnityNetworkFlagBase PossiblyBase = other.GetComponent<UnityNetworkFlagBase>();
        if (PossiblyBase && PossiblyBase.teamNumber != teamNumber)
        {
            Debug.Log("Im on Enemy Base");
            transform.SetParent(null);
            transform.position = teamBase.position;
        }

        TankMovement tank = other.GetComponent<TankMovement>();

        if (!tank || IsCaptured) return;


        if (tank.teamNumber == teamNumber.Value && transform.parent == null)
        {
            Debug.Log("Im returning to base");

            transform.position = teamBase.position;
            return;
        }

        transform.SetParent(other.transform);

        transform.position = other.transform.position;
    }
}
