using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCollector : MonoBehaviour
{
    public GameObject[] shells;
    public GameObject[] tanks = new GameObject[10];
    public GameObject[] mapObjects;
    public GameObject theTank;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (theTank)
        {
            Debug.LogWarning(theTank.GetComponent<Transform>().position);
        }

        //Debug.LogWarning(tanks.Length);
    }
}
