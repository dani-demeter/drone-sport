using DroneSport.Networking;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DroneSport.UI
{
    public class MenuUIController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private TMP_Text statusText;

        private void Awake()
        {
            hostButton.onClick.AddListener(OnHostClicked);
            joinButton.onClick.AddListener(OnJoinClicked);
        }

        private void OnEnable()
        {
            NetworkClient.OnDisconnectedEvent += OnDisconnected;
            NetworkClient.OnErrorEvent += OnError;
        }

        private void OnDisable()
        {
            NetworkClient.OnDisconnectedEvent -= OnDisconnected;
            NetworkClient.OnErrorEvent -= OnError;
        }

        private void OnHostClicked()
        {
            CachePlayerName();
            statusText.text = "";
            NetworkManager.singleton.StartHost();
        }

        private void OnJoinClicked()
        {
            CachePlayerName();
            string address = addressInputField.text.Trim();
            NetworkManager.singleton.networkAddress = string.IsNullOrEmpty(address) ? "localhost" : address;
            statusText.text = "Connecting...";
            NetworkManager.singleton.StartClient();
        }

        private void CachePlayerName()
        {
            string name = nameInputField.text.Trim();
            LocalPlayerSettings.PlayerName = string.IsNullOrEmpty(name) ? "Player" : name;
        }

        private void OnDisconnected()
        {
            statusText.text = "Disconnected";
        }

        private void OnError(TransportError error, string reason)
        {
            statusText.text = $"Connection failed: {reason}";
        }
    }
}
