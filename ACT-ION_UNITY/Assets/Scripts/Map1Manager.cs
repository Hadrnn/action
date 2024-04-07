using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    UnityNetwork = 1,
    SinglePlayerBot = 2,
    Empty = 3,
}
public class Map1Manager : MonoBehaviour
{

    public GameType Type = GameType.SinglePlayerBot;

    public GameObject UnityNetworkManager;
    public GameObject UnityNetworkMenu;
    public GameObject UnityNetworkEventSystem;

    public GameObject BotTank;
    public GameObject PlayerTank;

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
}
