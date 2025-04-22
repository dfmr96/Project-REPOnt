using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private GameObject moversPrefab;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private TMP_Text propProgressText;
    [SerializeField] private TMP_Text moversCapturedText;
    [SerializeField] private TMP_Text timerText;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void ShowEndResults(bool isMovers) 
    { 
        Instantiate(isMovers ? moversPrefab : ghostPrefab, canvasTransform);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackToMainMenu() { PhotonNetwork.Disconnect(); }

    public void ExitGame() 
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
    
    public void UpdatePropProgress(int placed, int total) { propProgressText.text += "\n" + $"{placed} / {total}"; }

    public void UpdateCapturedMovers(int actualMovers, int totalMovers) { moversCapturedText.text += "\n" + $"{actualMovers} / {totalMovers}"; }

    public void UpdateTimer(float timer)
    {
        if (timer <= 30) timerText.color = Color.red;
        timerText.text += "\n" + timer;
    }
}
