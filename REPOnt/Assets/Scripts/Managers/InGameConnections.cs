using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class InGameConnections : MonoBehaviourPunCallbacks
{
    public static InGameConnections Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }
    public override void OnDisconnected(DisconnectCause cause) { SceneManager.LoadScene("MainMenu"); }
}
