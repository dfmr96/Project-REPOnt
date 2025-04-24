using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject joinPanel;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CreateRoom()
    {
        ConnectionManager.Instance.CreateRoom(); 
        ConnectionManager.Instance.OnRoomCreated += OnRoomCreated;
    }

    private void OnRoomCreated() 
    { 
        hostPanel.SetActive(true);
        mainPanel.SetActive(false);
        hostPanel.GetComponent<HostPanel>().GetButtonId();
        ConnectionManager.Instance.OnRoomCreated -= OnRoomCreated;
    }

    public void JoinRoom() 
    {
        joinPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

}
