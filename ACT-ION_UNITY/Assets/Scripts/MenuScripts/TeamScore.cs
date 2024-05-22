using UnityEngine;
using TMPro;

public class TeamScore : MonoBehaviour
{
    public int teamNumber;

    private TextMeshProUGUI scoreText;
    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        //int stat;

        //if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamDeathMatch)
        //    stat = GameObject.Find("InfoCollector").GetComponent<InfoCollector>().teams[teamNumber].teamKills;
        //else
        //    stat = (int)GameObject.Find("InfoCollector").GetComponent<InfoCollector>().teams[teamNumber].teamStat;

        //scoreText.text = stat.ToString();
    }
}
