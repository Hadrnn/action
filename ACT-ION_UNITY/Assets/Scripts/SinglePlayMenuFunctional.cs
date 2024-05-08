using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayMenuFunctional : MenuFunctional
{

    public void Play()
    {
        GameSingleton.GetInstance().startedWithMenu = true;
        GameSingleton.GetInstance().currentGameType = GameSingleton.GameType.Single;

        UnityEngine.SceneManagement.SceneManager.LoadScene(map_index);
    }
}
