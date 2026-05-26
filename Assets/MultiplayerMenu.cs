using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI pingText;

    private Coroutine clientConnectRoutine;

    private void Awake()
    {
        // If Menu Panel was not assigned in the Inspector, use the scene Canvas
        if (menuPanel == null)
        {
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject != null)
            {
                menuPanel = canvasObject;
            }
        }
    }

    private void Start()
    {
        // Set the text to 0 at the very start
        if (playerCountText != null)
        {
            playerCountText.text = "Players Online: 0";
        }

        // Subscribe to events so the number updates when people join/leave
        NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerCount;
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdatePlayerCount;
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            return;
        }

        if (NetworkManager.Singleton.StartHost())
        {
            HideMenu();
            RefreshPlayerCount();
            Debug.Log("Host started. Waiting for clients on 127.0.0.1:7777");
        }
        else
        {
            Debug.LogWarning("Could not start Host.");
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            return;
        }

        if (!NetworkManager.Singleton.StartClient())
        {
            Debug.LogWarning("Could not start Client.");
            return;
        }

        Debug.Log("Connecting as Client... (start HOST on the other instance first)");

        if (clientConnectRoutine != null)
        {
            StopCoroutine(clientConnectRoutine);
        }

        clientConnectRoutine = StartCoroutine(WaitForClientConnection());
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            return;
        }

        if (NetworkManager.Singleton.StartServer())
        {
            HideMenu();
            RefreshPlayerCount();
        }
    }

    private IEnumerator WaitForClientConnection()
    {
        float timeout = 8f;

        while (timeout > 0f)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
            {
                HideMenu();
                Debug.Log("Client connected.");
                clientConnectRoutine = null;
                yield break;
            }

            timeout -= Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning(
            "Client did not connect in time. Make sure a HOST is already running, then press Start Client again.");
        ShowMenu();
        clientConnectRoutine = null;
    }

    private void HideMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void ShowMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
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

    private void Update()
    {
        if (pingText == null || NetworkManager.Singleton == null)
        {
            return;
        }

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            pingText.text = "Ping: --";
            return;
        }

        float ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(
            NetworkManager.Singleton.LocalClientId);
        pingText.text = "Ping: " + Mathf.RoundToInt(ping) + " ms";
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
