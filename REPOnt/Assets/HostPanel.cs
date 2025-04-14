using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HostPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI playersText;
    private void OnEnable() { ConnectionManager.Instance.OnRoomJoined += GetPlayers; }
    public void GetButtonId() { idText.text += ConnectionManager.Instance.GetRunId(); }
    private void GetPlayers(int quantity) { playersText.text = quantity + "/6"; }

    private void OnDisable() { ConnectionManager.Instance.OnRoomJoined -= GetPlayers; }
}
