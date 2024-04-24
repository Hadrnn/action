using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using Unity.Netcode.Transports.UTP;
using System.IO;
using System;
using System.Net.Sockets;
using System.Text;

public enum GameType
{
    UnityNetwork = 1,
    SinglePlayerBot = 2,
    Empty = 3,
    UnityServer = 4,
    UnityClient = 5,
    OnlyBots = 6,
}

public enum SpawnType
{
    Determined = 0,
    Randomized = 1,
}

public enum GameMode
{
    DeathMatch = 0,
    TeamDeathMatch = 1,
    CaptureTheFlag = 2,
    TeamBattle = 3,
    Domination = 4,
}
public class Map1Manager : NetworkBehaviour
{
    /// <summary>
    /// Regular: 0.02f
    /// </summary>
    public float tickTime = 0.02f;

    public GameType Type = GameType.SinglePlayerBot;
    public SpawnType ST = SpawnType.Determined;
    public GameMode Mode = GameMode.DeathMatch;
    public bool FriendlyFire = true;

    public GameObject UnityNetworkManager;
    public GameObject UnityNetworkMenu;
    public GameObject UnityNetworkEventSystem;

    public GameObject BotTank1;
    public GameObject BotTank2;

    /// <summary>
    /// Spawn position of bot 1 or team 1 spawn
    /// </summary>
    public Vector3 Pos1 = new Vector3(0f, 0f, 10f);
    /// <summary>
    /// Spawn position of bot 2 or team 2 spawn
    /// </summary>
    public Vector3 Pos2 = new Vector3(0f, 0f, -30f);
    public Vector3 PlayerPos = new Vector3(0f, 0f, -10f);

    public NetworkPrefabsList NetworkTankPrefabs;
    public NetworkPrefabsList NetworkMapObjects;


    public PlayerPrefabsList PlayerPrefabs;

    public GameObject FlagPrefab;
    public GameObject FlagBasePrefab;
    public GameObject CaptureBasePrefab;



    private string ServerAddress = "";
    private ushort ServerPort = 0;

    private const string ServerAddressMark = "address=";
    private const string ServerPortMark = "port=";
    private const string PythonServerAddress = "25.12.195.48";
    // EGOR "25.12.195.48"
    // TIMUR "25.56.145.143"

    private int ticks = 0;
    private bool DidSetFriendEnemy = false;
    private const int FriendEnemySetTick = 2;

    private const int FlagIndex = 0;
    private const int FlagBaseIndex = 1;
    private const int DominationBaseIndex = 2;

    private void Awake()
    {
        //Debug.Log(GameSingleton.GetInstance().startedWithMenu);

        GameSingleton.GetInstance().friendlyFire = FriendlyFire;

        if (!GameSingleton.GetInstance().startedWithMenu)
        switch (Mode)
        {
            case GameMode.TeamDeathMatch:
                GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.TeamDeathMatch;
                break;
            case GameMode.DeathMatch:
                GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.DeathMatch;
                break;
            case GameMode.CaptureTheFlag:
                GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.CaptureTheFlag;
                break;
            case GameMode.TeamBattle:
                GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.TeamBattle;
                break;
            case GameMode.Domination:
                GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.Domination;
                break;
            default:
                Debug.Log("MODE NOT IN START SWITCH");
                break;
        }

        if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamDeathMatch ||
            GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag ||
            GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamBattle)
        {
            InfoCollector collector = GetComponent<InfoCollector>();
            collector.team1Spawn = Pos1;
            collector.team2Spawn = Pos2;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        Time.fixedDeltaTime = tickTime;

        switch (Type)
        {
            case GameType.SinglePlayerBot:
                switch (GameSingleton.GetInstance().currentGameMode)
                {
                    case GameSingleton.GameMode.Domination:
                        Instantiate(CaptureBasePrefab, Pos1, Quaternion.Euler(-90, 0, 0));
                        Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Pos1, 30), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Pos1, 30), Quaternion.Euler(new Vector3(0f, 0f, 0f)));

                        break;
                    case GameSingleton.GameMode.CaptureTheFlag:
                        FlagCapture flag = Instantiate(FlagPrefab, SpawnManager.GetSpawnPos(Pos1, 30), Quaternion.Euler(-90, 0, 0)).GetComponent<FlagCapture>();
                        FlagBase flagBase = Instantiate(FlagBasePrefab, Pos1, Quaternion.Euler(-90, 0, 0)).GetComponent<FlagBase>();

                        flagBase.teamNumber = 0;
                        flag.teamNumber = 0;
                        flag.teamBase = flagBase.transform;

                        flag = Instantiate(FlagPrefab, SpawnManager.GetSpawnPos(Pos2, 30), Quaternion.Euler(-90, 0, 0)).GetComponent<FlagCapture>();
                        flagBase = Instantiate(FlagBasePrefab, Pos2, Quaternion.Euler(-90, 0, 0)).GetComponent<FlagBase>();

                        flagBase.teamNumber = 1;
                        flag.teamNumber = 1;
                        flag.teamBase = flagBase.transform;

                        Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Pos1, 30), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    case GameSingleton.GameMode.TeamDeathMatch:
                    case GameSingleton.GameMode.TeamBattle:
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Pos1, 30), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    case GameSingleton.GameMode.DeathMatch:
                        Instantiate(BotTank1, Pos1, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank2, Pos2, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], PlayerPos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    default:
                        break;
                }
                break;
            case GameType.UnityNetwork:
                GameObject manager = Instantiate(UnityNetworkManager);
                UnityTransport transport = manager.GetComponent<UnityTransport>();


                //// GET DATA FROM OUR PYTHON SERVER
                //transport.ConnectionData.Address = ServerAddress;
                //transport.ConnectionData.Port = ServerPort;

                Instantiate(UnityNetworkMenu);
                Instantiate(UnityNetworkEventSystem);
                break;
            case GameType.Empty:
                break;
            case GameType.OnlyBots:
                switch (ST)
                {
                    case SpawnType.Determined:
                        Instantiate(BotTank1, Pos1, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank2, Pos2, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    case SpawnType.Randomized:
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 30), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank2, SpawnManager.GetSpawnPos(Pos2, 30), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    default:
                        break;
                }
            
                break;
            case GameType.UnityServer:

                //string path = "C:/Users/bukin/Desktop/action/TestBuild/OutputTest.txt";
                //using (StreamWriter sw = new StreamWriter(path))

                string[] parameters = System.Environment.GetCommandLineArgs();
                HandleAnswer(parameters);
                Instantiate(UnityNetworkEventSystem);

                GameObject ServerManager = Instantiate(UnityNetworkManager);
                UnityTransport ServerTransport = ServerManager.GetComponent<UnityTransport>();
                ServerTransport.ConnectionData.Address = ServerAddress;
                ServerTransport.ConnectionData.Port = ServerPort;

                NetworkManager.Singleton.StartServer();

                if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag)
                {
                    UnityNetworkFlagBase flagBase = Instantiate(NetworkMapObjects.PrefabList[FlagBaseIndex].Prefab,
                        Pos1, Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagBase>();
                    UnityNetworkFlagCapture flag = Instantiate(NetworkMapObjects.PrefabList[FlagIndex].Prefab,
                        Pos1, Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagCapture>();

                    flagBase.teamNumber = new NetworkVariable<int>(0);
                    flag.teamNumber = new NetworkVariable<int>(0);
                    flag.teamBase = flagBase.transform;
                    flag.GetComponent<NetworkObject>().Spawn();
                    flagBase.GetComponent<NetworkObject>().Spawn();



                    flagBase = Instantiate(NetworkMapObjects.PrefabList[FlagBaseIndex].Prefab,
                        Pos2, Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagBase>();
                    flag = Instantiate(NetworkMapObjects.PrefabList[FlagIndex].Prefab,
                        Pos2, Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagCapture>();

                    flagBase.teamNumber = new NetworkVariable<int>(1);
                    flag.teamNumber = new NetworkVariable<int>(1);
                    flag.teamBase = flagBase.transform;
                    flag.GetComponent<NetworkObject>().Spawn();
                    flagBase.GetComponent<NetworkObject>().Spawn();

                }
                else if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.Domination)
                {
                    UnityNetworkBaseCapture captureBase = Instantiate(NetworkMapObjects.PrefabList[DominationBaseIndex].Prefab,
                        Vector3.zero, Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkBaseCapture>();

                    captureBase.GetComponent<NetworkObject>().Spawn();
                }


                break;
            case GameType.UnityClient:

                Instantiate(UnityNetworkEventSystem);
                GameObject ClientManager = Instantiate(UnityNetworkManager);
                UnityTransport ClientTransport = ClientManager.GetComponent<UnityTransport>();

                //// GET DATA FROM OUR PYTHON SERVER
                //// CHECK IF ADDRESS? PORT IS NOT EMPTY
                ///
                // Connect to server
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(PythonServerAddress, 9999);
                // Send message
                string message = "PlayerName=Player1 Map=Map1";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                socket.Send(messageBytes);
                // Recieve answer
                byte[] answer = new byte[2048];
                socket.Receive(answer);
                string d_answer = Encoding.UTF8.GetString(answer);
                socket.Close();

                // Answer handling
                HandleAnswer(d_answer.Split(' '));
                ClientTransport.ConnectionData.Address = ServerAddress;
                ClientTransport.ConnectionData.Port = ServerPort;

                NetworkManager.Singleton.StartClient();
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (!DidSetFriendEnemy)
        {
            if (ticks > FriendEnemySetTick)
            {
                GetComponent<InfoCollector>().SetFriendEnemy();
                GetComponent<InfoCollector>().SetBaseLights();
                //Debug.Log("Set friend enemy");
                DidSetFriendEnemy = true;
            }
            ++ticks;
        }
    }

    private void HandleAnswer(string[] parameters)
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            //sw.Write(parameters[i]);
            if (parameters[i].StartsWith(ServerAddressMark))
            {
                ServerAddress = parameters[i].Remove(0, ServerAddressMark.Length);
            }
            if (parameters[i].StartsWith(ServerPortMark))
            {
                ServerPort = Convert.ToUInt16(parameters[i].Remove(0, ServerPortMark.Length));
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Current tank is");
            Debug.Log(GameSingleton.GetInstance().currentTank);
            GameObject HostTank = Instantiate(NetworkTankPrefabs.PrefabList[GameSingleton.GetInstance().currentTank].Prefab);
            HostTank.GetComponent<NetworkObject>().Spawn();
            //HostTank.GetComponent<NetworkObject>().SpawnAsPlayerObject(0,true);
        }
        else
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, GameSingleton.GetInstance().currentTank);
        }
    }

    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientID, int tankID)
    {
        GameObject newPlayer = Instantiate(NetworkTankPrefabs.PrefabList[tankID].Prefab);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientID, true);
    }
}
