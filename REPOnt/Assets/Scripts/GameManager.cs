using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform prisonPoint;
    public Transform PrisonPoint => prisonPoint;
    private readonly List<MoverController> movers = new(); //readonly evita que te pisen la lista de mala leche

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

    
    
}
