public class SinglePlayMenuFunctional : MenuFunctional
{

    private void Awake()
    {
    }
    public void Play()
    {
        GameSingleton.GetInstance().startedWithMenu = true;
        GameSingleton.GetInstance().currentGameType = GameSingleton.GameType.Single;

        UnityEngine.SceneManagement.SceneManager.LoadScene(map_index);
    }

}
