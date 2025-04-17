using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button joinRandomButton;
    private void Start()
    {
        ConnectionManager.Instance.OnConnectToMaster += CheckButtons;
    }
    private void CheckButtons() 
    {
        hostButton.interactable = true;
        joinButton.interactable = true;
        joinRandomButton.interactable = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
