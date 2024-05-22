using System.Linq;
using UnityEngine;

public class TabMenu : MonoBehaviour
{
    public Transform Team;
    public Transform TankBar;

    public Transform horisontal;
    public Transform vertical;

    [HideInInspector]
    public static bool barsSet = false;

    public void OnEnable()
    {
        if (!barsSet) SetBars();
    }

    public void SetBars()
    {
        barsSet = true;

        while (horisontal.childCount > 0)
        {
            DestroyImmediate(horisontal.GetChild(0).gameObject);
        }
        while (vertical.childCount > 0)
        {
            DestroyImmediate(vertical.GetChild(0).gameObject);
        }


        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.DeathMatch)
        {
            for (int i = 0; i < collector.teams.Count; ++i)
            {
                for (int j = 0; j < collector.teams[i].tanks.Count; ++j)
                {
                    TankStatBar currentTank = Instantiate(TankBar).GetComponent<TankStatBar>();
                    currentTank.tank = collector.teams[i].tanks[j];
                    currentTank.transform.SetParent(vertical);
                }
            }
            return;
        }


        for (int i = 0; i< collector.teams.Count; ++i)
        {
            Transform currentTeam = Instantiate(Team) as Transform;
            currentTeam.SetParent(horisontal);
            for (int j = 0; j < collector.teams[i].tanks.Count; ++j) 
            {
                TankStatBar currentTank = Instantiate(TankBar).GetComponent<TankStatBar>();
                currentTank.tank = collector.teams[i].tanks[j];
                currentTank.transform.SetParent(currentTeam);
            }
            currentTeam.localScale = Vector3.one;
        }
    }

    public void Update()
    {
        var tanks = GameObject.FindGameObjectsWithTag("TankBar");
        var sorted = tanks.OrderBy(bar => bar.GetComponent<TankStatBar>().kills);

        foreach (var tankBar in sorted)
        {
            tankBar.transform.SetAsFirstSibling();
        }
    }
}
