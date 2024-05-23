using TMPro;
using UnityEngine;

public class TankStatBar : MonoBehaviour
{
    public TextMeshProUGUI KillsText;
    public TextMeshProUGUI DeathsText;
    public TextMeshProUGUI NameText;


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
        NameText.text = tank.name;
    }
}
