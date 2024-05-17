using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultyPlayMenuFunctional : MenuFunctional
{
    public void Play()
    {
        GameSingleton.GetInstance().startedWithMenu = true;
        GameSingleton.GetInstance().currentGameType = GameSingleton.GameType.Network;
        GameSingleton.GetInstance().currentMap = map_index;
        UnityEngine.SceneManagement.SceneManager.LoadScene(map_index);
    }
}
