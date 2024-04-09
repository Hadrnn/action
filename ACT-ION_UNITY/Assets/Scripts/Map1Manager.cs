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
}
public class Map1Manager : NetworkBehaviour
{
    public GameType Type = GameType.SinglePlayerBot;

    public GameObject UnityNetworkManager;
    public GameObject UnityNetworkMenu;
    public GameObject UnityNetworkEventSystem;

    public GameObject BotTank;
    public GameObject PlayerTank;

    public NetworkPrefabsList prefabs;

    private string ServerAddress = "";
    private ushort ServerPort = 0;

    private const string ServerAddressMark = "address=";
    private const string ServerPortMark = "port=";


    // Start is called before the first frame update
    void Start()
    {
        switch (Type)
        {
            case GameType.SinglePlayerBot:
                Instantiate(BotTank,new Vector3(0f,0f,10f),Quaternion.Euler(new Vector3(0f,0f,0f)));
                Instantiate(PlayerTank, new Vector3(0f, 0f, -10f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
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
                var message = "PlayerName=Qwerty Map=Map1";
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
            GameObject HostTank = Instantiate(prefabs.PrefabList[GameSingleton.GetInstance().currentTank].Prefab);
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
        GameObject newPlayer = Instantiate(prefabs.PrefabList[tankID].Prefab);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientID, true);
    }
}
