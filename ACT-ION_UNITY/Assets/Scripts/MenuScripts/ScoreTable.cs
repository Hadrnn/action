using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTable : MonoBehaviour
{
    public GameObject team0;
    public GameObject team1;


    void Start()
    {
        if (GameSingleton.GetInstance().currentGameMode != GameSingleton.GameMode.DeathMatch)
        {
            team0.SetActive(true);
            team1.SetActive(true);
        }
    }

}
