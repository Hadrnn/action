using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFunctional : MonoBehaviour
{
    protected int map_index = 1;
    protected int max_map_index = 2;
    protected int min_map_index = 1;
    protected int tank_index = 0;
    protected int max_tank_index = 3;
    protected int min_tank_index = 0;
    protected int changed_game_mode_index = 1;

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
            gameObject.GetComponentInChildren<Transform>().Find("ChoseMapMenu").GetComponentInChildren<Transform>().Find("SandMapPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseMapMenu").GetComponentInChildren<Transform>().Find("SandMapPct").gameObject.SetActive(false);
        }
        if (map_index == 2)
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseMapMenu").GetComponentInChildren<Transform>().Find("ForestMapPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseMapMenu").GetComponentInChildren<Transform>().Find("ForestMapPct").gameObject.SetActive(false);
        }






        if (tank_index == GameSingleton.Tanks.Tank)
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("TankPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("TankPct").gameObject.SetActive(false);
        }
        if (tank_index == GameSingleton.Tanks.APC)
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("APCPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("APCPct").gameObject.SetActive(false);
        }
        if (tank_index == GameSingleton.Tanks.HeavyTank)
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("HeavyPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("HeavyPct").gameObject.SetActive(false);
        }
        if (tank_index == GameSingleton.Tanks.Artillery)
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("ARTPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("ChoseTankMenu").GetComponentInChildren<Transform>().Find("ARTPct").gameObject.SetActive(false);
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
        changed_game_mode_index = 1;
        Debug.Log(changed_game_mode_index);

        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.TeamDeathMatch;
    }
    public void ChoseFreeForeAll()
    {
        changed_game_mode_index = 2;
        Debug.Log(changed_game_mode_index);

        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.DeathMatch;
    }
    public void ChoseTeamBattle()
    {
        changed_game_mode_index = 3;
        Debug.Log(changed_game_mode_index);

        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.TeamBattle;

    }
    public void ChoseCaptureTheFlag()
    {
        changed_game_mode_index = 4;
        Debug.Log(changed_game_mode_index);

        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.CaptureTheFlag;

    }
    public void ChoseHoldingPoints()
    {
        changed_game_mode_index = 5;
        Debug.Log(changed_game_mode_index);

        GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.Domination;
    }
}
