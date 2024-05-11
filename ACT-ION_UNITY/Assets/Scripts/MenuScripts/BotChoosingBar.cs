using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BotChoosingBar : MonoBehaviour
{
    public int team = 0;
    public int tankIndex = 0;
    public int tankAmount = 0;
    public TextMeshProUGUI text;

    private void OnEnable()
    {
        tankAmount = GameSingleton.GetInstance().botAmounts[team, tankIndex];
        text.text = tankAmount.ToString();
    }
    public void UpTankAmount()
    {
        ++tankAmount;
        text.text = tankAmount.ToString();
        GameSingleton.GetInstance().botAmounts[team,tankIndex] = tankAmount;
    }

    public void DownTankAmount()
    {
        if (tankAmount > 0) --tankAmount;
        text.text = tankAmount.ToString();
        GameSingleton.GetInstance().botAmounts[team, tankIndex] = tankAmount;
    }
}
