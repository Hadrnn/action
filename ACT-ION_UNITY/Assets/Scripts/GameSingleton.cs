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
        public const int Empty = 0;
        public const int Single = 1;
        public const int Network = 2;
        public const int Server = 3; 
    }

    public class Difficulty
    {
        public const int Easy = 0;
        public const int Medium = 1;
        public const int Hard = 2;
        public const int Insane = 3;
    }

    public class SceneName
    {
        public const string StartMenu = "StartMenu";
        public const string Map1 = "Map1";
        public const string Map2 = "Map2";
        public const string Map3 = "Map3";
    }

    static GameSingleton instance;

    public string ServerAddress = "";
    public ushort ServerPort = 0;

    public int currentTank = 0;
    public int currentGameMode = GameMode.DeathMatch;
    public int currentMap = 1;
    public int currentGameType = GameType.Empty;
    public int playerTeam = -1;
    public ulong playerClientID = 0;

    public string playerName = "Player1";

    public int[,] botAmounts = new int[2, 4];

    public bool friendlyFire = true;
    public bool startedWithMenu = false;
    public bool paused = false;
    
    private GameSingleton() {
        //Debug.Log("SPAWNED SINGLETON");
    }

    public static GameSingleton GetInstance()
    {
        if (instance == null)
        {
            instance = new GameSingleton();
            instance.currentTank = Random.Range(0, 4);
        }
        //Debug.Log("GOT TO SINGLETON");
        return instance;
    }
}
