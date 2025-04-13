using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinPanel : MonoBehaviour
{
    private void Start() { ConnectionManager.Instance.OnRoomJoinUpdated += CheckStatus; }
    public void OnInputEnd(string text) 
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("El codigo de la sala esta vacio");
            return;
        }
        if (text.Length != 6)
        {
            Debug.LogWarning("El codigo de la sala debe tener 6 caracteres");
            return;
        }
        Debug.Log("Intentando unirse con el codigo: " + text);
        ConnectionManager.Instance.JoinRoom(text);
    }
    private void CheckStatus(string status)
    {
        if (!string.IsNullOrEmpty(status)) Debug.LogWarning(status);
    }

    private void OnDisable() { ConnectionManager.Instance.OnRoomJoinUpdated -= CheckStatus; }
}
