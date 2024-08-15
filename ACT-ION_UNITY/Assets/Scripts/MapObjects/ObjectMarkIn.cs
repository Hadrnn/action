using UnityEngine;

public class ObjectMarkIn : MonoBehaviour
{
    void Start()
    {
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.mapObjects.Add(gameObject);
    }
    
}
