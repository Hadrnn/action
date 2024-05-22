using System;
using UnityEngine;

public class ServerStart : MonoBehaviour
{
    private const string ServerAddressMark = "address=";
    private const string ServerPortMark = "port=";
    private const string MapNumberMark = "map=";
    private const string GameModeMark = "mode=";


    public static int MapNumber = 1;

    private void Awake()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        HandleAnswer(parameters);

        GameSingleton.GetInstance().currentGameType = GameSingleton.GameType.Server;
        GameSingleton.GetInstance().startedWithMenu = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(MapNumber);
    }
    public static void HandleAnswer(string[] parameters)
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            //sw.Write(parameters[i]);
            if (parameters[i].StartsWith(ServerAddressMark))
            {
                GameSingleton.GetInstance().ServerAddress = parameters[i].Remove(0, ServerAddressMark.Length);
            }
            if (parameters[i].StartsWith(ServerPortMark))
            {
                GameSingleton.GetInstance().ServerPort = Convert.ToUInt16(parameters[i].Remove(0, ServerPortMark.Length));
            }
            if (parameters[i].StartsWith(MapNumberMark))
            {
                MapNumber = Convert.ToUInt16(parameters[i].Remove(0, MapNumberMark.Length));
            }
            if (parameters[i].StartsWith(GameModeMark))
            {
                GameSingleton.GetInstance().currentGameMode = Convert.ToUInt16(parameters[i].Remove(0, GameModeMark.Length));
            }
        }
    }
}

