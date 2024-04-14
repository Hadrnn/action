using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCapture : MonoBehaviour
{
    public int teamNumber = 0;

    private bool IsCaptured;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<TankMovement>() || IsCaptured) return;

        transform.SetParent(other.transform);

        transform.position = other.transform.position;
    }


}
