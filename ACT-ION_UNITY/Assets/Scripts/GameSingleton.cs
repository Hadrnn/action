using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class GameSingleton
{
    public class Tanks
    {
        public const int Tank = 0;
        public const int APC = 1;
        public const int HeavyTank = 2;
        public const int Artillery = 3;
    }
    public class GameMode
    {
        public const int DeathMatch = 0;
        public const int TeamDeathMatch = 1;
        public const int CaptureTheFlag = 2;
        public const int TeamBattle = 3;
        public const int Domination = 4;
    }

    public class GameType
    {
        public const int Empty = -1;
        public const int Single = 0;
        public const int Network = 1;
    }

    static GameSingleton instance;
    public int currentTank = 0;
    public int currentGameMode = GameMode.DeathMatch;
    public int currentGameType = GameType.Empty;
    public int playerTeam = -1;
    public bool friendlyFire = true;
    public bool startedWithMenu = false;
    public int[,] botAmounts = new int[2, 4];
    public bool paused = false;
    
    private GameSingleton() {
        //Debug.Log("SPAWNED SINGLETON");
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
