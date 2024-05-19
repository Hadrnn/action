using UnityEngine;

public class MenuFunctional : MonoBehaviour
{
    public Transform ChooseMapMenu;
    public Transform ChooseTankMenu;

    protected int map_index = 1;
    protected int max_map_index = 2;
    protected int min_map_index = 1;
    protected int tank_index = 0;
    protected int max_tank_index = 3;
    protected int min_tank_index = 0;

    public void UpMapIndex()
    {
        if (map_index < max_map_index)
        {
            map_index++;
        }
        else
        {
            map_index = min_map_index;
        }
    }
    public void DownMapIndex()
    {
        if (map_index > min_map_index)
        {
            map_index--;
        }
        else
        {
            map_index = max_map_index;
        }
    }


    public void Update()
    {
        if (map_index == 1)
        {
            ChooseMapMenu.Find("SandMapPct").gameObject.SetActive(true);
        }
        else
        {
            ChooseMapMenu.Find("SandMapPct").gameObject.SetActive(false);
        }
        if (map_index == 2)
        {
            ChooseMapMenu.Find("ForestMapPct").gameObject.SetActive(true);
        }
        else
        {
            ChooseMapMenu.Find("ForestMapPct").gameObject.SetActive(false);
        }

        GameSingleton.GetInstance().currentTank = tank_index;
    }

    public void UpTankIndex()
    {
        if (tank_index < max_tank_index)
        {
            tank_index++;
        }
        else
        {
            tank_index = min_tank_index;
        }
    }
    public void DownTankIndex()
    {
        if (tank_index > min_tank_index)
        {
            tank_index--;
        }
        else
        {
            tank_index = max_tank_index;
        }
    }

    public void ChoseTeamDeathMatch()
    {
        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.TeamDeathMatch;
        Debug.Log("TDM");
    }
    public void ChoseFreeForeAll()
    {
        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.DeathMatch;
        Debug.Log("FFA");
    }
    public void ChoseTeamBattle()
    {
        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.TeamBattle;
        Debug.Log("TB");
    }
    public void ChoseCaptureTheFlag()
    {
        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.CaptureTheFlag;
        Debug.Log("CTF");
    }
    public void ChoseHoldingPoints()
    {
        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.Domination;
        Debug.Log("HP");
    }
}
