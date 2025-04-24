using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HostPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI playersText;

    private void OnEnable()
    {
        ConnectionManager.Instance.OnRoomJoined += UpdatePlayerCount; 
        DisplayRoomId();
    }

    public void DisplayRoomId()
    {
        if (idText != null)
        {
            idText.text = $"Room ID: {ConnectionManager.Instance.GetRoomId()}";
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
        ConnectionManager.Instance.OnRoomJoined -= UpdatePlayerCount;
    }
}
