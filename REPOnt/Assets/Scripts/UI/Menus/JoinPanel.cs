using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button joinButton;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TMP_Text joinText;
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private GameObject rootPanel;

    
    private const string JoiningLabel = "Joining...";
    private const string JoinLabel = "Join";
    private void OnEnable()
    {
        ConnectionManager.Instance.OnRoomJoinUpdated += HandleJoinStatus;
        ConnectionManager.Instance.OnRoomJoined += UpdatePlayerCount;
        
        ResetUI();
        leaveRoomButton.interactable = PhotonNetwork.InRoom;
    }
    public void OnInputEnd() 
    {
        string roomCode = input.text.Trim();

        if (!IsValidRoomCode(roomCode, out string errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        ClearError();
        joinText.text = JoiningLabel;
        ConnectionManager.Instance.JoinRoom(roomCode);
    }
    private void HandleJoinStatus(string status)
    {
        if (!string.IsNullOrEmpty(status))
        {
            if (status == "Joined")
            {
                input.interactable = false;
                joinButton.interactable = false;
                joinText.text = JoinLabel;
                ShowSuccess("Connection Successful!");
                
                leaveRoomButton.interactable = true;
            }
            else
            {
                Debug.LogWarning(status);
                ShowError(status);
                input.interactable = true;
                joinButton.interactable = true;
                joinText.text = JoinLabel;
                
                leaveRoomButton.interactable = false;
            }
        }
    }

    private void UpdatePlayerCount(int quantity)
    {
        if (playersText != null)
        {
            playersText.text = $"Players: {quantity} / 6";
        }
    }

    private void OnDisable() 
    { 
        ConnectionManager.Instance.OnRoomJoinUpdated -= HandleJoinStatus;
        ConnectionManager.Instance.OnRoomJoined -= UpdatePlayerCount;
    }
    
    private bool IsValidRoomCode(string code, out string errorMessage)
    {
        if (string.IsNullOrEmpty(code))
        {
            errorMessage = "Room code must not be empty.";
            return false;
        }

        if (code.Length != 6)
        {
            errorMessage = "Room code must be 6 characters long.";
            return false;
        }

        errorMessage = null;
        return true;
    }
    
    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = Color.red;
            input.Select();
        }
    }
    
    private void ShowSuccess(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = Color.green;
        }
    }
    
    private void ClearError()
    {
        if (errorText != null)
        {
            errorText.text = string.Empty;
        }
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        if (joinPanel != null)
            joinPanel.SetActive(false);
        
        leaveRoomButton.interactable = false;
        rootPanel.SetActive(true);
    }
    private void ResetUI()
    {
        input.text = "";
        input.interactable = true;

        playersText.text = "Players: 0 / 6";
    
        ClearError();
        joinText.text = JoinLabel;

        joinButton.interactable = true;
        leaveRoomButton.interactable = false;
    }
}
