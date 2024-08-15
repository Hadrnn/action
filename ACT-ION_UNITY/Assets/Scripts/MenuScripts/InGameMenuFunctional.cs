using Unity.Netcode;
using UnityEngine;

public class InGameMenuFunctional : MonoBehaviour
{
    private bool tab_active = false;
    private bool esc_active = false;

    private float savedTimeScale = 1f;

    private void Start()
    {
        savedTimeScale = Time.timeScale;
    }
    void Update()
    {
        tab_active = Input.GetKey(KeyCode.Tab);

        gameObject.GetComponentInChildren<Transform>().Find("TabMenu").gameObject.SetActive(tab_active);


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            esc_active = !esc_active;
            if (esc_active)
            {
                Time.timeScale = 0;

                gameObject.GetComponentInChildren<Transform>().Find("EscapeMenu").gameObject.SetActive(true);
                gameObject.GetComponentInChildren<Transform>().Find("EscapeMenu").Find("EscapeMenuButtons").gameObject.SetActive(true);
            }
            else
            {
                Time.timeScale = savedTimeScale;

                gameObject.GetComponentInChildren<Transform>().Find("EscapeMenu").gameObject.SetActive(false);
                gameObject.GetComponentInChildren<Transform>().Find("EscapeMenu").Find("EscapeMenuButtons").gameObject.SetActive(false);
                gameObject.GetComponentInChildren<Transform>().Find("EscapeMenu").Find("SoundSettingsMenu").gameObject.SetActive(false);

            }
        }
    }

    public void BackToStartMenu()
    {
        if(NetworkManager.Singleton)
        {
            NetworkManager.Singleton.Shutdown();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameSingleton.SceneName.StartMenu);
    }
}
