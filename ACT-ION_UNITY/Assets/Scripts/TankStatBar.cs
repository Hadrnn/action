using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TankStatBar : MonoBehaviour
{
    public TextMeshProUGUI KillsText;
    public TextMeshProUGUI DeathsText;

    public int kills;

    [HideInInspector] public InfoCollector.Team.TankHolder tank;

    private void Awake()
    {
    }

    private void Update()
    {

        kills = tank.kills;
        KillsText.text = tank.kills.ToString();
        DeathsText.text = tank.deaths.ToString();
    }
}
