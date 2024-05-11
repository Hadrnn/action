using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour
{
    public Image chooseMode;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnChoose);
    }
    public void OnChoose()
    {
        chooseMode.sprite = GetComponent<Image>().sprite;
    }
}
