using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCollector : MonoBehaviour
{
    public List<GameObject> shells = new();
    public List<GameObject> tanks = new();
    public List<GameObject> mapObjects = new();

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log((shells.Count, tanks.Count, mapObjects.Count));
    }
}
