using System.Linq;
using UnityEngine;
using static InfoCollector;

public class TabMenu : MonoBehaviour
{
    public Transform Team;
    public Transform TankBar;

    public Transform horisontal;
    public Transform vertical;

    private bool barsSet = false;

    public void OnEnable()
    {
        if (!barsSet) SetBars();
    }

    public void SetBars()
    {
        barsSet = true;
        Debug.Log("SetBars");

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

        Debug.Log("Setting bars");


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
