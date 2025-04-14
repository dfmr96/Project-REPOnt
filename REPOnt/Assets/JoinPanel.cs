using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TextMeshProUGUI playersText;
    private void OnEnable()
    {
        ConnectionManager.Instance.OnRoomJoinUpdated += CheckStatus;
        ConnectionManager.Instance.OnRoomJoined += GetPlayers;
    }
    public void OnInputEnd() 
    {
        if (string.IsNullOrEmpty(input.text))
        {
            Debug.LogWarning("El codigo de la sala esta vacio");
            return;
        }
        if (input.text.Length != 6)
        {
            Debug.LogWarning("El codigo de la sala debe tener 6 caracteres");
            return;
        }
        Debug.Log("Intentando unirse con el codigo: " + input.text);
        ConnectionManager.Instance.JoinRoom(input.text);
    }
    private void CheckStatus(string status)
    {
        if (!string.IsNullOrEmpty(status)) Debug.LogWarning(status);
        input.text = status;
    }

    private void GetPlayers(int quantity) { playersText.text = quantity + "/6"; }

    private void OnDisable() 
    { 
        ConnectionManager.Instance.OnRoomJoinUpdated -= CheckStatus;
        ConnectionManager.Instance.OnRoomJoined -= GetPlayers;
    }
}
