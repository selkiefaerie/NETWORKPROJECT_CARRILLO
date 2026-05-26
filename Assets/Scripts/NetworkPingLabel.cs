using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkPingLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pingText;

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
}
