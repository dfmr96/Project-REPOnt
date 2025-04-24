using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("End Game Screens")]
    [SerializeField] private GameObject moversPrefab;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform canvasTransform;
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text propProgressText;
    [SerializeField] private TMP_Text moversCapturedText;
    [SerializeField] private TMP_Text timerText;
    
    [SerializeField] private float warningTimeThreshold = 30f;
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Unity Methods
    // ──────────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // Public UI Updates
    // ──────────────────────────────────────────────────────────────────────────────
    
    public void UpdatePropProgress(int placed, int total) { propProgressText.text = $"Objects remaining\n{placed} / {total}"; }

    public void UpdateCapturedMovers(int actualMovers, int totalMovers) { moversCapturedText.text = $"Movers captured\n{actualMovers} / {totalMovers}"; }

    public void UpdateTimer(float timer)
    {
        if (timer <= warningTimeThreshold) timerText.color = Color.red;
        
        TimeSpan time = TimeSpan.FromSeconds(timer);
        timerText.text = $"Remaining Time\n{time:mm\\:ss}";
    }
    
    // ──────────────────────────────────────────────────────────────────────────────
    // UI Flow Control
    // ──────────────────────────────────────────────────────────────────────────────
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
}
