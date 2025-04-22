using System;
using System.Collections.Generic;
using PlayerScripts;
using Props;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform prisonPoint;

    private PhotonView photonView;
    private float timeForDisconnect = 8f;
    public Transform PrisonPoint => prisonPoint;


    private readonly List<MoverController> movers = new(); //readonly evita que te pisen la lista de mala leche
    [field: SerializeField] private List<DropZone> dropZones;
    [field: SerializeField] private List<PickupObject> pickupObjects;

    public List<DropZone> DropZones => dropZones;

    public List<PickupObject> PickupObjects => pickupObjects;

    [SerializeField] private int propsToWin = 3;
    [SerializeField] private int propsPlaced = 0;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        UIManager.Instance.UpdatePropProgress(propsPlaced, propsToWin);
    }

    public void RegisterMover(MoverController mover)
    {
        if (!movers.Contains(mover)) movers.Add(mover);
    }

    public void CheckMoversCaptured()
    {
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
        UIManager.Instance.UpdatePropProgress(propsPlaced, propsToWin);
        CheckDropCompletion();
    }
    
    public void RegisterPickupObject(PickupObject pickupObject)
    {
        if (!PickupObjects.Contains(pickupObject))
            PickupObjects.Add(pickupObject);
    }
    
    private bool PropsToWinReached()
    {
        return propsPlaced >= propsToWin;
    }

    [PunRPC]
    private void RPC_EndGame(bool isMover) { UIManager.Instance.ShowEndResults(isMover); }
}
