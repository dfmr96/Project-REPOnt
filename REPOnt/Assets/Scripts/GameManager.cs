using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using Props;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform prisonPoint;
    public Transform PrisonPoint => prisonPoint;
    private readonly List<MoverController> movers = new(); //readonly evita que te pisen la lista de mala leche
    private readonly List<DropZone> dropZones = new();
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }
    
    public void RegisterMover(MoverController mover)
    {
        if (!movers.Contains(mover))
        {
            movers.Add(mover);
        }
    }

    public void CheckMoversCaptured()
    {
        if (AreAllMoversCaptured())
        {
            Debug.Log("All movers have been captured!");
            //TODO Agregar lógica para RPC de victoria Ghost
        }
    }
    private bool AreAllMoversCaptured()
    {
        foreach (var mover in movers)
        {
            if (!mover.IsCaptured)
                return false;
        }

        return true;
    }
    public void RegisterDropZone(DropZone zone)
    {
        if (!dropZones.Contains(zone))
            dropZones.Add(zone);
    }
    public void CheckDropCompletion()
    {
        if (AreAllDropZonesPlaced())
        {
            Debug.Log("All objects have been placed! Movers win!");
            // TODO: Add Mover victory RPC/event
        }
    }
    
    private bool AreAllDropZonesPlaced()
    {
        //TODO Minimo de dropzones para ganar
        foreach (var zone in dropZones)
        {
            if (!zone.IsPlaced)
                return false;
        }

        return true;
    }
    
}
