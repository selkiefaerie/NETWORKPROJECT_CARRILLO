using UnityEngine;
using Unity.Netcode;
using TMPro;

public class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private TextMeshProUGUI playerCountText; 

    private void Start()
    {
        // Set the text to 0 at the very start
        if(playerCountText != null) playerCountText.text = "Players Online: 0";

        // Subscribe to events so the number updates when people join/leave
        NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerCount;
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdatePlayerCount;
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton.IsListening) return;

        if (NetworkManager.Singleton.StartHost())
        {
            HideMenu();
            RefreshPlayerCount();
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.IsListening) return;

        if (NetworkManager.Singleton.StartClient())
        {
            HideMenu();
        }
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.IsListening) return;

        if (NetworkManager.Singleton.StartServer())
        {
            HideMenu();
            RefreshPlayerCount();
        }
    }

    private void HideMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void UpdatePlayerCount(ulong id)
    {
        RefreshPlayerCount();
    }

    private void RefreshPlayerCount()
    {
        if (playerCountText != null && NetworkManager.Singleton != null)
        {
            int count = NetworkManager.Singleton.ConnectedClients.Count;
            playerCountText.text = "Players Online: " + count;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks or errors when changing scenes
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= UpdatePlayerCount;
            NetworkManager.Singleton.OnClientDisconnectCallback -= UpdatePlayerCount;
        }
    }
}