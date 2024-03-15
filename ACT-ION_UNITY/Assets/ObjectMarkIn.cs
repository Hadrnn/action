using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMarkIn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.mapObjects.Add(gameObject);
    }
    
}
