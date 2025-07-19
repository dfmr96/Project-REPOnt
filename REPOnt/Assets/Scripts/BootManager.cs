using UnityEngine;

public class BootManager : MonoBehaviour
{
    private void Awake() { GameAnalyticsHandler.Initialize(); }
}
