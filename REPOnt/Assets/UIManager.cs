using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private GameObject moversPrefab;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform canvasTransform;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void ShowEndResults(bool isMovers) { Instantiate(isMovers ? moversPrefab : ghostPrefab, canvasTransform); }
}
