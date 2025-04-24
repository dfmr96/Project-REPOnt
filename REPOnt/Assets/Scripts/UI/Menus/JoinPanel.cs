using System.Collections;
using System.Collections.Generic;
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
    private void OnEnable()
    {
        ConnectionManager.Instance.OnRoomJoinUpdated += HandleJoinStatus;
        ConnectionManager.Instance.OnRoomJoined += UpdatePlayerCount;
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
                ShowSuccess("Conectado con éxito!");
            }
            else
            {
                Debug.LogWarning(status);
                ShowError(status);
                input.interactable = true;
                joinButton.interactable = true;
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
            errorMessage = "El código de la sala está vacío.";
            return false;
        }

        if (code.Length != 6)
        {
            errorMessage = "El código debe tener 6 caracteres.";
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
}
