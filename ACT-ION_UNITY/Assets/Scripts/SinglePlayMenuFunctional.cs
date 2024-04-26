using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayMenuFunctional : MenuFunctional
{

    public void Play()
    {
        GameSingleton.GetInstance().startedWithMenu = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(map_index);
    }
}
