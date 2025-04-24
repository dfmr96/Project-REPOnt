using System;
using System.Collections.Generic;
using PlayerScripts;
using Props;
using UnityEngine;
using Photon.Pun;
using Unity.Collections;

public class GameManager : MonoBehaviour
{
    //Static
    public static GameManager Instance { get; private set; }
    
    //Serialized Fields
    [Header("Match Settings")]

    [SerializeField] private int propsToWin = 3;
    [SerializeField] private float matchDurationSeconds = 120;
    [SerializeField] private Transform prisonPoint;
    [field: SerializeField] private List<DropZone> dropZones;
    [field: SerializeField] private List<PickupObject> pickupObjects;
    
    [Header("Debug Info")]
    [SerializeField, ReadOnly] private int propsPlaced = 0;
    [SerializeField, ReadOnly] private float matchStartTime;
    [SerializeField, ReadOnly] private int capturedMovers = 0;
    
    //Internal references
    private PhotonView photonView;
    private readonly List<MoverController> movers = new();
    private bool matchStarted = false;

    //Properties
    private float TimeRemaining => Mathf.Max(0f, matchDurationSeconds - (float)(PhotonNetwork.Time - matchStartTime));
    public Transform PrisonPoint => prisonPoint;
    public List<DropZone> DropZones => dropZones;
    public List<PickupObject> PickupObjects => pickupObjects;
    public int PropsToWin => propsToWin;
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Unity Methods
    // ──────────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) 
            photonView.RPC(nameof(RPC_StartMatch), RpcTarget.All, (float)PhotonNetwork.Time);
        
    }
    private void Update()
    {
        if (!matchStarted) return;

        UpdateTimer();
    }
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Registration Methods
    // ──────────────────────────────────────────────────────────────────────────────

    public void RegisterMover(MoverController mover)
    {
        if (!movers.Contains(mover)) movers.Add(mover);
    }
    public void RegisterCapturedMover()
    {
        capturedMovers++;
        photonView.RPC(nameof(RPC_UpdateCapturedPlayers), RpcTarget.All);
        if (!PhotonNetwork.IsMasterClient) return;
        CheckMoversCaptured();
    }
    public void RegisterDropZone(DropZone zone)
    {
        if (!DropZones.Contains(zone)) DropZones.Add(zone);
    }
    public void RegisterPropPlaced()
    {
        propsPlaced++;
        UIManager.Instance.UpdatePropProgress(propsPlaced, propsToWin);
        Debug.Log($"[GameManager] RegisterPropPlaced called. {propsPlaced}/{PropsToWin}");
        CheckDropCompletion();
    }
    public void RegisterPickupObject(PickupObject pickupObject)
    {
        if (!PickupObjects.Contains(pickupObject))
            PickupObjects.Add(pickupObject);
    }

    
    // ──────────────────────────────────────────────────────────────────────────────
    // Game Logic
    // ──────────────────────────────────────────────────────────────────────────────
    
    private void UpdateTimer()
    {
        UIManager.Instance.UpdateTimer(TimeRemaining);

        if (PhotonNetwork.IsMasterClient && TimeRemaining <= 0f)
            EndGame(false);
    }
    private void CheckMoversCaptured()
    {
        if (!AreAllMoversCaptured()) return;
        Debug.Log("All movers have been captured!");
        EndGame(false);
    }
    private bool AreAllMoversCaptured()
    {
        int requiredToCapture = Mathf.CeilToInt(movers.Count / 2f);
        int actualCaptured = 0;

        foreach (var mover in movers)
        {
            if (mover.IsCaptured)
                actualCaptured++;
        }
        
        return actualCaptured >= requiredToCapture;
    }
    private void CheckDropCompletion()
    {
        if (!HasPropsToWinReached()) return;
        Debug.Log("All objects have been placed! Movers win!");
        EndGame(true);
    }

    private void EndGame(bool isMoverWinner)
    {
        photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, isMoverWinner);
    }
    private bool HasPropsToWinReached() { return propsPlaced >= PropsToWin; }

    // ──────────────────────────────────────────────────────────────────────────────
    // RPCs
    // ──────────────────────────────────────────────────────────────────────────────
    [PunRPC]
    private void RPC_EndGame(bool isMover) { UIManager.Instance.ShowEndResults(isMover); }

    [PunRPC]
    private void RPC_StartMatch(float startTime)
    {
        matchStartTime = startTime;
        matchStarted = true;
        photonView.RPC(nameof(RPC_UpdateCapturedPlayers), RpcTarget.All);
        UIManager.Instance.UpdatePropProgress(propsPlaced, propsToWin);
    }
    [PunRPC]
    private void RPC_UpdateCapturedPlayers() { UIManager.Instance.UpdateCapturedMovers(capturedMovers, PhotonNetwork.PlayerList.Length - 1); }

    // ──────────────────────────────────────────────────────────────────────────────
    // Helper Methods
    // ──────────────────────────────────────────────────────────────────────────────
    
    public MoverController GetMoverByViewID(int viewID)
    {
        foreach (var mover in movers)
        {
            if (mover.TryGetComponent(out PhotonView view) && view.ViewID == viewID)
                return mover;
        }
        return null;
    }

}
