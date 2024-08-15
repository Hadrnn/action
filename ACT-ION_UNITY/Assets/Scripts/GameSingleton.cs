using System.IO;
using System.Text;
using UnityEngine;

public class GameSingleton
{
    public class Volume
    {
        public float masterLvl = 1;
        public float musicLvl = 1;
        public float sfxLvl = 1;
        public float engineLvl = 1;
    }
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



    private static GameSingleton instance;
    private static string iniFilePath = "ini.txt";
    private const int attributeNameLength = 49;
    private const int attributeValueLength = 30;
    private const int iniLineLength = attributeNameLength + 2 + attributeValueLength; // +2 for '=' and '\n'

    // DONT USE SPACE IN MARKS !!!!  
    public const string masterVolumeMark = "masterVolume";
    public const string musicVolumeMark = "musicVolume";
    public const string sfxVolumeMark = "sfxVolume";
    public const string engineVolumeMark = "engineVolume";
    public const string playerNameMark = "playerName";
    public const string allMark = "-ALL-";


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

    public Volume currentVolume;
    //public bool paused = false;
    
    private GameSingleton() {
        //Debug.Log("SPAWNED SINGLETON");
        currentVolume = new Volume();
    }

    public static GameSingleton GetInstance()
    {
        if (instance == null)
        {
            instance = new GameSingleton();
            instance.GetIni();
            instance.currentTank = UnityEngine.Random.Range(0, 4);
        }
        //Debug.Log("GOT TO SINGLETON");
        return instance;
    }

    public void ChangeIni(string mark, string value)
    {
        if (mark == allMark)
        {
            if (File.Exists(iniFilePath)) File.Delete(iniFilePath);

            CreateIni();
            return;
        }


        using (FileStream fs = File.Open(iniFilePath, FileMode.Open))
        {

            byte[] buff = new byte[iniLineLength];
            UTF8Encoding encoding = new UTF8Encoding(true);
            int readLen;
            long pointerPos = fs.Position;


            while ((readLen = fs.Read(buff, 0, buff.Length)) > 0)
            {

                //Debug.Log(pointerPos);

                string currentLine = encoding.GetString(buff, 0, readLen);

                if (currentLine.StartsWith(mark))
                {
                    //Debug.Log("Found mark");

                    fs.Seek(pointerPos, SeekOrigin.Begin);
                    string line = mark.PadRight(attributeNameLength, ' ') + '=' + value.PadRight(attributeValueLength, ' ') + '\n';
                    WriteToFile(fs, line);
                }
                //Debug.Log("--|" + currentLine + "|---");

                pointerPos = fs.Position;
            }
        }
    }

    private void CreateIni()
    {
        using (FileStream fs = File.Create(iniFilePath))
        {
            Debug.LogWarning("Created a ini file");

            string line;


            line = playerNameMark.PadRight(attributeNameLength, ' ') + '=' +
                playerName.PadRight(attributeValueLength, ' ') + '\n';
            WriteToFile(fs, line);



            //// VOLUME 
            line = masterVolumeMark.PadRight(attributeNameLength,' ') + '=' + 
                currentVolume.masterLvl.ToString("0.000").PadRight(attributeValueLength,' ') + '\n';
            WriteToFile(fs, line);

            line = musicVolumeMark.PadRight(attributeNameLength, ' ') + '=' +
                currentVolume.musicLvl.ToString("0.000").PadRight(attributeValueLength, ' ') + '\n';
            WriteToFile(fs, line);

            line = sfxVolumeMark.PadRight(attributeNameLength, ' ') + '=' +
                currentVolume.sfxLvl.ToString("0.000").PadRight(attributeValueLength, ' ') + '\n';
            WriteToFile(fs, line);

            line = engineVolumeMark.PadRight(attributeNameLength, ' ') + '=' +
                currentVolume.engineLvl.ToString("0.000").PadRight(attributeValueLength, ' ') + '\n';
            WriteToFile(fs, line);
            //// -------------


            fs.Flush();
        }
    }

    private static void WriteToFile(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }

    public void GetIni()
    {
        if (!File.Exists(iniFilePath))
        {
            CreateIni();
        }

        string[] lines = File.ReadAllLines(iniFilePath);

        foreach (string line in lines)
        {
            string mark = line.Substring(0, line.IndexOf(' '));
            string value = line.Substring(attributeNameLength + 1);

            //Debug.Log("--|" + mark + "|--");
            //Debug.Log(value);

            switch (mark)
            {
                case masterVolumeMark:
                    currentVolume.masterLvl = System.Convert.ToSingle(value);
                    //Debug.Log("Master Volume");
                    break;
                case musicVolumeMark:
                    currentVolume.musicLvl = System.Convert.ToSingle(value);
                    //Debug.Log("Music Volume");
                    break;
                case sfxVolumeMark:
                    currentVolume.sfxLvl = System.Convert.ToSingle(value);
                    //Debug.Log("SFX Volume");
                    break;
                case engineVolumeMark:
                    currentVolume.engineLvl = System.Convert.ToSingle(value);
                    //Debug.Log("Engine Volume");
                    break;
                case playerNameMark:
                    playerName = value.Substring(0, value.IndexOf(" "));
                    break;
                default:
                    break;
            }
        };
    }
}
