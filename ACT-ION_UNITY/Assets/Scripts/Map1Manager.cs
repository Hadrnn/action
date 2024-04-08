using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking.Transport.Error;

public enum GameType
{
    UnityNetwork = 1,
    SinglePlayerBot = 2,
    Empty = 3,
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


    private bool needToSpawn = true;

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
                Instantiate(UnityNetworkManager);
                Instantiate(UnityNetworkMenu);
                Instantiate(UnityNetworkEventSystem);
                break;
            case GameType.Empty:
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (!NetworkManager.Singleton || !needToSpawn) return;

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Current tank is");
            Debug.Log(GameSingleton.GetInstance().currentTank);
            GameObject HostTank = Instantiate(prefabs.PrefabList[GameSingleton.GetInstance().currentTank].Prefab);
            HostTank.GetComponent<NetworkObject>().Spawn();
            needToSpawn = false;
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
