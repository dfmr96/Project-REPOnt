using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HostPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button startGameButton;
    
    private const int RequiredPlayers = 3;


    private void OnEnable()
    {
        ConnectionManager.Instance.OnRoomJoined += HandlePlayerCount; 
        DisplayRoomId();
        UpdateUI(PhotonNetwork.CurrentRoom.PlayerCount);
    }
    private void OnDisable()
    {
        ConnectionManager.Instance.OnRoomJoined -= HandlePlayerCount;
    }

    public void DisplayRoomId()
    {
        if (idText != null)
        {
            idText.text = $"Room ID: {ConnectionManager.Instance.GetRoomId()}";
        }
    }
    
    public void CopyRoomIdToClipboard()
    {
        string roomId = ConnectionManager.Instance.GetRoomId();
        GUIUtility.systemCopyBuffer = roomId;
        Debug.Log($"Room ID copied to clipboard: {roomId}");
    }

    private void HandlePlayerCount(int quantity)
    {
        UpdateUI(quantity);
    }
    
    private void UpdateUI(int quantity)
    {
        if (playersText != null)
            playersText.text = $"Players: {quantity} / 6";

        if (startGameButton != null)
            startGameButton.interactable = quantity >= RequiredPlayers;

        if (statusText != null)
        {
            statusText.text = quantity >= RequiredPlayers
                ? "Ready to start!"
                : $"Waiting for {RequiredPlayers - quantity} more player(s)...";
            statusText.color = quantity >= RequiredPlayers ? Color.green : Color.yellow;
        }
    }
    
    public void OnClickStartGame()
    {
        ConnectionManager.Instance.StartGame();
    }
}
