using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayMenuFunctional : MenuFunctional
{

    private void Awake()
    {
        //GameSingleton.GetInstance().botAmounts = new int[2, 4];
    }
    public void Play()
    {
        GameSingleton.GetInstance().startedWithMenu = true;
        GameSingleton.GetInstance().currentGameType = GameSingleton.GameType.Single;

        UnityEngine.SceneManagement.SceneManager.LoadScene(map_index);
    }

}
