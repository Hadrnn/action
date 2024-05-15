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
using UnityEngine.UIElements;

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
public class MapManager : NetworkBehaviour
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

    public Vector3[] Positions = { new Vector3(0f, 0f, 10f), new Vector3(0f, 0f, -30f), new Vector3(0f, 0f, 10f) };

    public NetworkPrefabsList NetworkTankPrefabs;
    public NetworkPrefabsList NetworkMapObjects;


    public PlayerPrefabsList PlayerPrefabs;
    public PlayerPrefabsList BotPrefabs;

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
    private const int FriendEnemySetTick = 50;

    private const int FlagIndex = 0;
    private const int FlagBaseIndex = 1;
    private const int DominationBaseIndex = 2;

    private const int amountOfTanks = 4;
    private const int amountOfTeams = 2;

    private const int defaultSpawnRadius = 10;

    private struct TeamForNet : INetworkSerializable
    {
        public int teamNumber;
        public int teamStat;
        public int teamKills;
        public int alivePlayers;
        public NetTankHolder[] tanks;

        public TeamForNet(int teamNumber_, int teamStat_, int teamKills_, int alivePlayers_, int tankCount)
        {
            teamNumber = teamNumber_;
            teamStat = teamStat_;
            teamKills = teamKills_;
            alivePlayers = alivePlayers_;
            tanks = new NetTankHolder[tankCount];
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref teamNumber);
            serializer.SerializeValue(ref teamStat);
            serializer.SerializeValue(ref teamKills);
            serializer.SerializeValue(ref alivePlayers);
            serializer.SerializeValue(ref tanks);
        }

        public struct NetTankHolder : INetworkSerializable
        {
            public ulong ID;
            public int kills;
            public int deaths;
            public string name;

            public NetTankHolder(ulong ID_, int kills_, int deaths_, string name_)
            {
                ID = ID_;
                kills = kills_;
                deaths = deaths_;
                name = name_;
            }
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ID);
                serializer.SerializeValue(ref kills);
                serializer.SerializeValue(ref deaths);
                serializer.SerializeValue(ref name);
            }
        }
    }

    private void Awake()
    {
        //Debug.Log(GameSingleton.GetInstance().startedWithMenu);

        GameSingleton.GetInstance().friendlyFire = FriendlyFire;

        if (!GameSingleton.GetInstance().startedWithMenu)
        {
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
        }
        else
        {
            switch (GameSingleton.GetInstance().currentGameType)
            {
                case GameSingleton.GameType.Network:
                    Type = GameType.UnityClient;
                    break;
                case GameSingleton.GameType.Empty:
                    Type = GameType.Empty;
                    break;
                case GameSingleton.GameType.Single:
                    Type = GameType.SinglePlayerBot;
                    break;
                default:
                    break;
            }
        }


        if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamDeathMatch ||
            GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag ||
            GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamBattle)
        {
            InfoCollector collector = GetComponent<InfoCollector>();
            collector.team1Spawn = Positions[0];
            collector.team2Spawn = Positions[1];
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        Time.fixedDeltaTime = tickTime;

        TankMovement PlayerTank;
        InfoCollector collector = GetComponent<InfoCollector>();

        switch (Type)
        {
            case GameType.SinglePlayerBot:
                switch (GameSingleton.GetInstance().currentGameMode)
                {
                    case GameSingleton.GameMode.Domination:

                        collector.objectives.Add(Instantiate(CaptureBasePrefab, Positions[2], Quaternion.Euler(-90, 0, 0)));

                        for (ushort teamNumber = 0; teamNumber < amountOfTeams; ++teamNumber)
                        {
                            for (ushort tankIndex = 0; tankIndex < amountOfTanks; ++tankIndex)
                            {
                                int tankAmount = GameSingleton.GetInstance().botAmounts[teamNumber, tankIndex];
                                for (ushort i = 0; i < tankAmount; ++i)
                                {
                                    TankMovement tank = Instantiate(BotPrefabs.PrefabList[tankIndex],
                                        SpawnManager.GetSpawnPos(Positions[teamNumber], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                                    tank.teamNumber = teamNumber;
                                }
                            }
                        }

                        PlayerTank = Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank],
                            SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                        PlayerTank.teamNumber = 0;

                        //Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        //Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f)));

                        break;
                    case GameSingleton.GameMode.CaptureTheFlag:

                        collector = GetComponent<InfoCollector>();

                        for (ushort teamNumber = 0; teamNumber < amountOfTeams; ++teamNumber)
                        {
                            FlagBase flagBase = Instantiate(FlagBasePrefab, Positions[teamNumber], Quaternion.Euler(-90, 0, 0)).GetComponent<FlagBase>();
                            FlagCapture flag = Instantiate(FlagPrefab, SpawnManager.GetSpawnPos(Positions[teamNumber], defaultSpawnRadius), Quaternion.Euler(-90, 0, 0)).GetComponent<FlagCapture>();

                            flagBase.teamNumber = teamNumber;
                            flag.teamNumber = teamNumber;
                            flag.teamBase = flagBase.transform;

                            flag.transform.position = flagBase.transform.position;

                            collector.objectives.Add(flag.gameObject);
                            collector.objectives.Add(flagBase.gameObject);

                        }

                        // Spawning bots
                        for (ushort teamNumber = 0; teamNumber < amountOfTeams; ++teamNumber)
                        {
                            for (ushort tankIndex = 0; tankIndex < amountOfTanks; ++tankIndex)
                            {
                                int tankAmount = GameSingleton.GetInstance().botAmounts[teamNumber, tankIndex];
                                for (ushort i = 0; i < tankAmount; ++i)
                                {
                                    TankMovement BotTank = Instantiate(BotPrefabs.PrefabList[tankIndex],
                                        SpawnManager.GetSpawnPos(Positions[teamNumber], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                                    BotTank.teamNumber = teamNumber;
                                }
                            }
                        }



                        PlayerTank = Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank],
                            SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                        PlayerTank.teamNumber = 0;
                        
                        //Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    case GameSingleton.GameMode.TeamDeathMatch:

                        // No need to uncomment cuz it will fall into TeamBattle code 


                        //for (ushort teamNumber = 0; teamNumber < amountOfTeams; ++teamNumber)
                        //{
                        //    for (ushort tankIndex = 0; tankIndex < amountOfTanks; ++tankIndex)
                        //    {
                        //        int tankAmount = GameSingleton.GetInstance().botAmounts[teamNumber, tankIndex];
                        //        for (ushort i = 0; i < tankAmount; ++i)
                        //        {
                        //            TankMovement tank = Instantiate(BotPrefabs.PrefabList[tankIndex],
                        //                SpawnManager.GetSpawnPos(Positions[teamNumber], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                        //            tank.teamNumber = teamNumber;
                        //        }
                        //    }
                        //}

                        //Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f)));

                        //break;
                    case GameSingleton.GameMode.TeamBattle:

                        for (ushort teamNumber = 0; teamNumber < amountOfTeams; ++teamNumber)
                        {
                            for (ushort tankIndex = 0; tankIndex < amountOfTanks; ++tankIndex)
                            {
                                int tankAmount = GameSingleton.GetInstance().botAmounts[teamNumber, tankIndex];
                                for (ushort i = 0; i < tankAmount; ++i)
                                {
                                    TankMovement tank = Instantiate(BotPrefabs.PrefabList[tankIndex],
                                        SpawnManager.GetSpawnPos(Positions[teamNumber], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                                    tank.teamNumber = teamNumber;
                                }
                            }
                        }

                        PlayerTank = Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank],
                            SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                        PlayerTank.teamNumber = 0;

                        //Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f)));

                        //Instantiate(BotTank1, SpawnManager.GetSpawnPos(Positions[0], 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        //Instantiate(BotTank1, SpawnManager.GetSpawnPos(Positions[0], 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        //Instantiate(BotTank1, SpawnManager.GetSpawnPos(Positions[0], 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        //Instantiate(BotTank1, SpawnManager.GetSpawnPos(Positions[0], 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        //Instantiate(BotTank1, SpawnManager.GetSpawnPos(Positions[0], 40), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    case GameSingleton.GameMode.DeathMatch:

                        for (ushort teamNumber = 0; teamNumber < amountOfTeams; ++teamNumber)
                        {
                            for (ushort tankIndex = 0; tankIndex < amountOfTanks; ++tankIndex)
                            {
                                int tankAmount = GameSingleton.GetInstance().botAmounts[teamNumber, tankIndex];
                                for (ushort i = 0; i < tankAmount; ++i)
                                {
                                    Instantiate(BotPrefabs.PrefabList[tankIndex], SpawnManager.GetSpawnPos(Positions[teamNumber], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                                }
                            }
                        }

                        //Instantiate(BotTank1, Positions[0], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        //Instantiate(BotTank2, Positions[1], Quaternion.Euler(new Vector3(0f, 0f, 0f)));

                        PlayerTank = Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank],
                            SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f))).GetComponent<TankMovement>();
                        PlayerTank.teamNumber = 0;
                        //Instantiate(PlayerPrefabs.PrefabList[GameSingleton.GetInstance().currentTank], Positions[2], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
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
                //Instantiate(UnityNetworkEventSystem);
                break;
            case GameType.Empty:
                break;
            case GameType.OnlyBots:
                switch (ST)
                {
                    case SpawnType.Determined:
                        Instantiate(BotTank1, Positions[0], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank2, Positions[1], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        break;
                    case SpawnType.Randomized:
                        Instantiate(BotTank1, SpawnManager.GetSpawnPos(Positions[0], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                        Instantiate(BotTank2, SpawnManager.GetSpawnPos(Positions[1], defaultSpawnRadius), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
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
                //Instantiate(UnityNetworkEventSystem);

                GameObject ServerManager = Instantiate(UnityNetworkManager);
                UnityTransport ServerTransport = ServerManager.GetComponent<UnityTransport>();
                ServerTransport.ConnectionData.Address = ServerAddress;
                ServerTransport.ConnectionData.Port = ServerPort;

                NetworkManager.Singleton.StartServer();

                if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag)
                {
                    UnityNetworkFlagBase flagBase = Instantiate(NetworkMapObjects.PrefabList[FlagBaseIndex].Prefab,
                        Positions[0], Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagBase>();
                    UnityNetworkFlagCapture flag = Instantiate(NetworkMapObjects.PrefabList[FlagIndex].Prefab,
                        Positions[0], Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagCapture>();

                    flagBase.teamNumber = new NetworkVariable<int>(0);
                    flag.teamNumber = new NetworkVariable<int>(0);
                    flag.teamBase = flagBase.transform;
                    flag.GetComponent<NetworkObject>().Spawn();
                    flagBase.GetComponent<NetworkObject>().Spawn();



                    flagBase = Instantiate(NetworkMapObjects.PrefabList[FlagBaseIndex].Prefab,
                        Positions[1], Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagBase>();
                    flag = Instantiate(NetworkMapObjects.PrefabList[FlagIndex].Prefab,
                        Positions[1], Quaternion.Euler(-90, 0, 0)).GetComponent<UnityNetworkFlagCapture>();

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

                //Instantiate(UnityNetworkEventSystem);
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
                if (NetworkManager.Singleton)
                {
                    SyncTeamsServerRpc();
                }

                AddNetworkManagerCallbacks();
                GetComponent<InfoCollector>().SetFriendEnemy();
                GetComponent<InfoCollector>().SetBaseLights();

                //Debug.Log("Set friend enemy");
                DidSetFriendEnemy = true;
            }
            ++ticks;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void SyncTeamsServerRpc()
    {
        SyncTeamsClientRpc(GetTeamsForSync());
    }

    private TeamForNet[] GetTeamsForSync()
    {
        List<InfoCollector.Team> teams = GetComponent<InfoCollector>().teams;
        TeamForNet[] teamsForNet = new TeamForNet[teams.Count];

        for (ushort i = 0; i < teams.Count; ++i)
        {
            TeamForNet currentTeam = new TeamForNet(teams[i].teamNumber,
                teams[i].teamStat, teams[i].teamKills, teams[i].alivePlayers, teams[i].tanks.Count);

            for (ushort j = 0; j < teams[i].tanks.Count; ++j)
            {
                currentTeam.tanks[j] = new TeamForNet.NetTankHolder(teams[i].tanks[j].tank.GetComponent<UnityNetworkTankHealth>().OwnerClientId,
                    teams[i].tanks[j].kills, teams[i].tanks[j].deaths, teams[i].tanks[j].name);
            }

            teamsForNet[i] = currentTeam;
        }

        return teamsForNet;
    }

    [ClientRpc]
    private void SyncTeamsClientRpc(TeamForNet[] netTeams)
    {
        List<InfoCollector.Team> newCollectorTeams = new();

        List<GameObject> tanks = new();

        List<InfoCollector.Team> oldCollectorTeams = GetComponent<InfoCollector>().teams;

        foreach(InfoCollector.Team currentTeam in oldCollectorTeams)
        {
            foreach(InfoCollector.Team.TankHolder currentHolder in currentTeam.tanks)
            {
                tanks.Add(currentHolder.tank);
            }
        }

        foreach (TeamForNet currentTeam in netTeams)
        {
            InfoCollector.Team newTeam = new InfoCollector.Team(
                currentTeam.teamNumber, currentTeam.teamKills, currentTeam.teamStat, currentTeam.alivePlayers);

            //Debug.Log("Team number " + currentTeam.teamNumber.ToString() + " with tanks: " + currentTeam.tanks.Length.ToString());

            foreach(TeamForNet.NetTankHolder holder in currentTeam.tanks)
            {
                //Debug.Log("Tank with id " + holder.ID.ToString() + " deahts: " + holder.deaths.ToString());
                foreach (GameObject tank in tanks)
                {
                    UnityNetworkTankShooting currentShooting = tank.GetComponent<UnityNetworkTankShooting>();
                    if (currentShooting.OwnerClientId == holder.ID)
                    {
                        currentShooting.tankHolder = new InfoCollector.Team.TankHolder(tank, holder.name);
                        currentShooting.tankHolder.deaths = holder.deaths;
                        currentShooting.tankHolder.kills = holder.kills;
                        currentShooting.tankHolder.team = newTeam;

                        newTeam.tanks.Add(currentShooting.tankHolder);
                        if(currentShooting.OwnerClientId == OwnerClientId)
                        {
                            Debug.Log("Found Myself");
                            GameSingleton.GetInstance().playerTeam = currentTeam.teamNumber;
                        } 
                        continue;
                    }
                }
            }
            newCollectorTeams.Add(newTeam);
        }


        GetComponent<InfoCollector>().teams = newCollectorTeams;
        GetComponent<InfoCollector>().SetFriendEnemy();
        TabMenu.barsSet = false;
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

    private void AddNetworkManagerCallbacks()
    {
        var netMan = NetworkManager.Singleton;
        if (netMan != null)
        {
            Debug.Log("Registering callbacks");
            RemoveNetworkManagerCallbacks();

            netMan.OnClientDisconnectCallback += OnClientDisconnect;
        }
        else
        {
            Debug.LogWarning("Manager singleton is not active, did not register callbacks");
        }
    }

    private void RemoveNetworkManagerCallbacks()
    {
        var netMan = NetworkManager.Singleton;
        if (netMan != null)
        {
            netMan.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (IsClient) return;

        List<InfoCollector.Team> teams = GetComponent<InfoCollector>().teams;

        for (ushort i = 0; i < teams.Count; ++i)
        {

            for (ushort j = 0; j < teams[i].tanks.Count; ++j)
            {
                if (teams[i].tanks[j].tankID == clientId)
                {
                    Debug.Log("Found leaving player");
                    teams[i].tanks.RemoveAt(j);

                    SyncTeamsClientRpc(GetTeamsForSync());
                    TabMenu.barsSet = false;
                    return;
                }
            }
        }
    }
}
    