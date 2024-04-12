using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotArtTurretTurning : BotTurretTurning
{
    //new public Transform FireTransform;
    // Start is called before the first frame update
    private void Start()
    {
        Gun = gameObject.GetComponentInParent<BotShooting>();

        //teamNumber = gameObject.GetComponentInParent<TankMovement>().teamNumber;

        collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        BotTransform = gameObject.GetComponentInParent<TankMovement>().gameObject.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
