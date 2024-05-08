using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuFunctional : MonoBehaviour
{
    private bool tab_active = false;
    private bool esc_active = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tab_active = !tab_active;
            if (tab_active)
            {
                gameObject.GetComponentInChildren<Transform>().Find("TabMenu").gameObject.SetActive(true);
            }
            else
            {
                gameObject.GetComponentInChildren<Transform>().Find("TabMenu").gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            esc_active = !esc_active;
            if (esc_active)
            {
                gameObject.GetComponentInChildren<Transform>().Find("EscapeMenu").gameObject.SetActive(true);
            }
            else
            {
                gameObject.GetComponentInChildren<Transform>().Find("EscapeMenu").gameObject.SetActive(false);
            }
        }
    }

    public void BackToStartMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
