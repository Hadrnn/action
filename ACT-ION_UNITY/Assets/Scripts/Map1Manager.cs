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
    Domitanion = 4,
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

    public NetworkPrefabsList NetworkPrefabs;
    public PlayerPrefabsList PlayerPrefabs;

    private string ServerAddress = "";
    private ushort ServerPort = 0;

    private const string ServerAddressMark = "address=";
    private const string ServerPortMark = "port=";

    private int ticks = 0;
    private bool DidSetFriendEnemy = false;
    private const int FriendEnemySetTick = 2;

    private void Awake()
    {
        GameSingleton.GetInstance().friendlyFire = FriendlyFire;

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
            case GameMode.Domitanion:
                GameSingleton.GetInstance().currentGameMode = GameSingleton.GameMode.Domination;
                break;
            default:
                Debug.Log("MODE NOT IN START SWITCH");
                break;
        }

        if (Mode == GameMode.TeamDeathMatch ||
           Mode == GameMode.CaptureTheFlag ||
           Mode == GameMode.TeamBattle)
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
                switch (Mode)
                {
                    case GameMode.TeamDeathMatch:
                    case GameMode.TeamBattle:
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Pos1, 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Pos1, 30), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    case GameMode.DeathMatch:
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

                string[] parameters = System.Environment.GetCommandLineArgs();
                //string path = "C:/Users/bukin/Desktop/action/TestBuild/OutputTest.txt";
                //using (StreamWriter sw = new StreamWriter(path))
                HandleAnswer(parameters);

                Instantiate(UnityNetworkEventSystem);

                GameObject ServerManager = Instantiate(UnityNetworkManager);
                UnityTransport ServerTransport = ServerManager.GetComponent<UnityTransport>();
                ServerTransport.ConnectionData.Address = ServerAddress;
                ServerTransport.ConnectionData.Port = ServerPort;
                //ServerTransport.ConnectionData.Port = 9000;


                NetworkManager.Singleton.StartServer();
                break;
            case GameType.UnityClient:
                Instantiate(UnityNetworkEventSystem);
                GameObject ClientManager = Instantiate(UnityNetworkManager);
                UnityTransport ClientTransport = ClientManager.GetComponent<UnityTransport>();

                //// GET DATA FROM OUR PYTHON SERVER
                //// CHECK IF ADDRESS? PORT IS NOT EMPTY
                // Коннект к серверу
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect("25.12.195.48", 9999);
                // Отправка сообщения с требованиями игры
                var message = "PlayerName=Player1 Map=Map1";
                var messageBytes = Encoding.UTF8.GetBytes(message);
                socket.Send(messageBytes);
                // Получения ответа для коннекта к нужной игре
                var answer = new byte[2048];
                socket.Receive(answer);
                var d_answer = Encoding.UTF8.GetString(answer);
                socket.Close();
                // Обработка ответа
                HandleAnswer(d_answer.Split(' '));
                ClientTransport.ConnectionData.Address = ServerAddress;
                ClientTransport.ConnectionData.Port = ServerPort;
                // ClientTransport.ConnectionData.Address = "25.12.195.48";
                // ClientTransport.ConnectionData.Port = 9111;

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
        //if (NetworkManager.Singleton) GetComponent<InfoCollector>().SetFriendEnemyNetwork();
        //else GetComponent<InfoCollector>().SetFriendEnemy();
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
            GameObject HostTank = Instantiate(NetworkPrefabs.PrefabList[GameSingleton.GetInstance().currentTank].Prefab);
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
        GameObject newPlayer = Instantiate(NetworkPrefabs.PrefabList[tankID].Prefab);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientID, true);
    }
}
