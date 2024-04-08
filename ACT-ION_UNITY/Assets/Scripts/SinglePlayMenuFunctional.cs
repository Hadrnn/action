using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayMenuFunctional : MonoBehaviour
{
    private int map_index = 0;
    private int max_map_index = 0;
    private int min_map_index = 0;
    private int tank_index = 0;
    private int max_tank_index = 3;
    private int min_tank_index = 0;
    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(map_index);
    }
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
        if(map_index == 0)
        {
            gameObject.GetComponentInChildren<Transform>().Find("MapPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("MapPct").gameObject.SetActive(false);
        }
        if (tank_index == 0)
        {
            gameObject.GetComponentInChildren<Transform>().Find("TankPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("TankPct").gameObject.SetActive(false);
        }
        if (tank_index == 1)
        {
            gameObject.GetComponentInChildren<Transform>().Find("APCPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("APCPct").gameObject.SetActive(false);
        }
        if (tank_index == 2)
        {
            gameObject.GetComponentInChildren<Transform>().Find("ARTPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("ARTPct").gameObject.SetActive(false);
        }
        if (tank_index == 3)
        {
            gameObject.GetComponentInChildren<Transform>().Find("HeavyPct").gameObject.SetActive(true);
        }
        else
        {
            gameObject.GetComponentInChildren<Transform>().Find("HeavyPct").gameObject.SetActive(false);
        }
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
        Debug.Log(tank_index);
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
        Debug.Log(tank_index);
    }


}
