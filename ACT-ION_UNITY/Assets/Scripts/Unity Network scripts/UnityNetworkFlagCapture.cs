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


        if (PossiblyBase && PossiblyBase.teamNumber.Value != teamNumber.Value)
        {
            Debug.Log("Im on Enemy Base");
            //GetComponent<NetworkObject>().TryRemoveParent();
            transform.SetParent(null);
            SetParentClientRpc(-1,-1);

            transform.position = teamBase.position;
            IsCaptured = false;
            return;
        }


        UnityNetworkTankMovement tank = other.GetComponent<UnityNetworkTankMovement>();
        InfoCollector.Team.TankHolder holder = GetComponent<UnityNetworkTankShooting>().tankHolder;


        if (!tank || IsCaptured) return;


        if (tank.teamNumber == teamNumber.Value && transform.parent == null)
        {
            Debug.Log("Im returning to base");

            transform.position = teamBase.position;
            IsCaptured = false;
            return;
        }

        //GetComponent<NetworkObject>().TrySetParent(other.transform, true); ;
        transform.SetParent(other.transform);
        SetParentClientRpc(holder.team.teamNumber, holder.tankID);


        //transform.position = Vector3.zero;
        transform.position = other.transform.position;
        IsCaptured = true;
    }

    [ClientRpc]
    private void SetParentClientRpc(int teamNumber, int tankID)
    {
        if ((teamNumber == -1) && (tankID == -1))
        {
            transform.SetParent(null);
            return;
        }

        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        foreach (InfoCollector.Team.TankHolder tankHolder in collector.teams[teamNumber].tanks)
        {
            if (tankHolder.tankID == tankID)
            {
                Debug.Log("FOUND HIM");
                transform.SetParent(tankHolder.tank.transform);
                break;
            }
        }
    }
}
