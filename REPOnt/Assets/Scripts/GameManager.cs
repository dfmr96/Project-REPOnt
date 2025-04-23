using System;
using System.Collections.Generic;
using PlayerScripts;
using Props;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private int propsToWin = 3;
    [SerializeField] private int propsPlaced = 0;
    [SerializeField] private float matchDurationSeconds = 120;
    [SerializeField] private Transform prisonPoint;
    [field: SerializeField] private List<DropZone> dropZones;
    [field: SerializeField] private List<PickupObject> pickupObjects;

    private PhotonView photonView;
    private float matchStartTime;
    private int capturedMovers = 0;
    private readonly List<MoverController> movers = new(); //readonly evita que te pisen la lista de mala leche

    public Transform PrisonPoint => prisonPoint;
    public List<DropZone> DropZones => dropZones;
    public List<PickupObject> PickupObjects => pickupObjects;
    public int PropsToWin => propsToWin;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) 
            photonView.RPC("RPC_SetMatchStartTime", RpcTarget.All, (float)PhotonNetwork.Time);
        photonView.RPC("RPC_UpdateCapturedPlayers", RpcTarget.All);
        UIManager.Instance.UpdatePropProgress(propsPlaced, PropsToWin);
    }

    private void Update()
    {
        if (matchStartTime == 0) return;

        float timeRemaining = Mathf.Max(0f, matchDurationSeconds - (float)(PhotonNetwork.Time - matchStartTime));

        UIManager.Instance.UpdateTimer(timeRemaining);

        if (PhotonNetwork.IsMasterClient && timeRemaining <= 0f)
            photonView.RPC("RPC_EndGame", RpcTarget.All, false);
    }

    public void RegisterMover(MoverController mover)
    {
        if (!movers.Contains(mover)) movers.Add(mover);
    }

    public void RegisterCapturedMover()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        capturedMovers++;
        photonView.RPC("RPC_UpdateCapturedPlayers", RpcTarget.All);
        CheckMoversCaptured();
    }

    public void CheckMoversCaptured()
    {
        photonView.RPC("RPC_UpdateCapturedPlayers", RpcTarget.All);
        if (AreAllMoversCaptured())
        {
            Debug.Log("All movers have been captured!");
            photonView.RPC("RPC_EndGame", RpcTarget.All, false);
        }
    }
    private bool AreAllMoversCaptured()
    {
        for (int i = 0; i <= movers.Count - (movers.Count / 2); i++)
        {
            if (!movers[i].IsCaptured) return false;
        }
        return true;
    }
    public void RegisterDropZone(DropZone zone)
    {
        if (!DropZones.Contains(zone)) DropZones.Add(zone);
    }

    private void CheckDropCompletion()
    {
        if (!PropsToWinReached()) return;
        Debug.Log("All objects have been placed! Movers win!");
        photonView.RPC("RPC_EndGame", RpcTarget.All, true);
    }

    public void RegisterPropPlaced()
    {
        propsPlaced++;
        UIManager.Instance.UpdatePropProgress(propsPlaced, PropsToWin);
        CheckDropCompletion();
    }
    
    public void RegisterPickupObject(PickupObject pickupObject)
    {
        if (!PickupObjects.Contains(pickupObject))
            PickupObjects.Add(pickupObject);
    }
    
    private bool PropsToWinReached() { return propsPlaced >= PropsToWin; }

    [PunRPC]
    private void RPC_EndGame(bool isMover) { UIManager.Instance.ShowEndResults(isMover); }
    [PunRPC]
    private void RPC_SetMatchStartTime(float startTime) { matchStartTime = startTime; }
    [PunRPC]
    private void RPC_UpdateCapturedPlayers() { UIManager.Instance.UpdateCapturedMovers(capturedMovers, PhotonNetwork.PlayerList.Length - 1); }
}
