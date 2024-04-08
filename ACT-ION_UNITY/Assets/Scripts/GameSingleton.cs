using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSingleton
{
    public static class Tanks
    {
        public const int Tank = 0;
        public const int APC = 1;
        public const int HeavyTank = 2;
        public const int Artillery = 3;
    }

    static GameSingleton instance;
    public int currentTank = 0;
    
    private GameSingleton() {
        Debug.Log("SPAWNED SINGLETON");
    }

    public static GameSingleton GetInstance()
    {
        if (instance == null)
        {
            instance = new GameSingleton();
        }
        //Debug.Log("GOT TO SINGLETON");
        return instance;
    }


}
