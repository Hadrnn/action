using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class UnityNetworkFlagCapture : NetworkBehaviour
{
    public NetworkVariable<int> teamNumber;
    public Transform teamBase;

    private NetworkVariable<bool> IsCaptured;

    private void Awake()
    {
        IsCaptured = new NetworkVariable<bool>(false);
    }

    private void Start()
    {
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");

        foreach (GameObject CTFBase in bases)
        {
            UnityNetworkFlagBase currBase = CTFBase.GetComponent<UnityNetworkFlagBase>();

            if (currBase.teamNumber.Value == teamNumber.Value)
            {
                //Debug.Log("Flag found a base");
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
            //Debug.Log("Im on Enemy Base");
            transform.SetParent(null);
            SetParentClientRpc(0, 0, true, true);

            transform.position = teamBase.position;
            transform.rotation = Quaternion.Euler(-90, 0, 0);
            IsCaptured.Value = false;
            return;
        }


        UnityNetworkTankShooting tank = other.GetComponent<UnityNetworkTankShooting>();


        if (!tank || IsCaptured.Value)
            return;
        if (tank.GetComponent<UnityNetworkTankHealth>().m_Dead)
            return;

        if (tank.tankHolder.team.teamNumber == teamNumber.Value && transform.parent == null)
        {
            //Debug.Log("Im returning to base");

            transform.position = teamBase.position;
            transform.rotation = Quaternion.Euler(-90, 0, 0);
            IsCaptured.Value = false;
            return;
        }


        transform.SetParent(other.transform);
        SetParentClientRpc(tank.tankHolder.team.teamNumber, tank.tankHolder.tankID);

        transform.position = other.transform.position;
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        IsCaptured.Value = true;

    }

    public void SetCaptured(bool toSet)
    {
        IsCaptured.Value = toSet;
    }

    [ClientRpc]
    public void SetParentClientRpc(int teamNumber, ulong tankID,
        bool setNullParent = false, bool returnToBase = false)
    {
        if (setNullParent)
        {
            transform.SetParent(null);

            if (returnToBase)
            {
                transform.position = teamBase.position;
                transform.rotation = Quaternion.Euler(-90, 0, 0);
            }

            return;
        }

        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        foreach (InfoCollector.Team.TankHolder tankHolder in collector.teams[teamNumber].tanks)
        {
            if (tankHolder.tankID == tankID)
            {
                //Debug.Log("FOUND HIM");
                transform.SetParent(tankHolder.tank.transform);
                transform.position = tankHolder.tank.transform.position;
                transform.rotation = Quaternion.Euler(-90, 0, 0);
                break;
            }
        }
    }
}
