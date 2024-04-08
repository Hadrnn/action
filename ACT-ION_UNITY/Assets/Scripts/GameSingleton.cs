using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSingleton
{
    static GameSingleton instance;
    public int value = 0;
    private GameSingleton() {
        Debug.Log("SPAWNED SINGLETON");
    }

    public static GameSingleton GetInstance()
    {
        if (instance == null)
        {
            instance = new GameSingleton();
        }
        Debug.Log("GOT TO SINGLETON");
        return instance;
    }


}
