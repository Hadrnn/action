using UnityEngine;

public class FlagCapture : MonoBehaviour
{
    public int teamNumber = 0;
    public Transform teamBase;

    public bool IsCaptured;

    private void Start()
    {

        IsCaptured = false;

        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");

        foreach (GameObject CTFBase in bases)
        {
            FlagBase currBase = CTFBase.GetComponent<FlagBase>();

            if (currBase.teamNumber == teamNumber)
            {
                //Debug.Log("Flag found a base");
                teamBase = currBase.transform;
                break;
            }
        }

        if (teamBase.GetComponent<FlagBase>().teamNumber != teamNumber)
        {
            throw new System.Exception("NOT MATHCING TEAM NUMBER OF FLAG AND FLAG BASE");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enter");

        FlagBase PossiblyBase = other.GetComponent<FlagBase>();
        if (PossiblyBase && PossiblyBase.teamNumber != teamNumber)
        {
            //Debug.Log("Im on Enemy Base");
            transform.SetParent(null);
            transform.position = teamBase.position;
            IsCaptured = false;

            ++GameObject.Find("InfoCollector").GetComponent<InfoCollector>().teams[PossiblyBase.teamNumber].teamStat;
        }

        TankMovement tank = other.GetComponent<TankMovement>();

        if (!tank || IsCaptured) return;

        //Debug.Log("Im tank");

        if (tank.teamNumber == teamNumber && transform.parent == null)
        {
            //Debug.Log("Im returning to base");
            
            transform.position = teamBase.position;
            IsCaptured = false;

            return;
        }

        transform.SetParent(other.transform);

        transform.position = other.transform.position;

        IsCaptured = true;
    }


}
