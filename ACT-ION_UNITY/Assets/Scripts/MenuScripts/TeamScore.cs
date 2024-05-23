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
        int stat;

        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        if (!collector.teamsSet) return;

        if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.TeamDeathMatch)
            stat = collector.teams[teamNumber].teamKills;
        else
            stat = (int)collector.teams[teamNumber].teamStat;

        scoreText.text = stat.ToString();
    }
}
