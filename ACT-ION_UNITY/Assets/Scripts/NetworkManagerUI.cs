using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(call:(() => 
        {
            NetworkManager.Singleton.StartServer();
        }));

        hostButton.onClick.AddListener(call:(() => 
        {
            NetworkManager.Singleton.StartHost();
        }));

        clientButton.onClick.AddListener(call:(() => 
        {
            NetworkManager.Singleton.StartClient();
        }));
    }
}
