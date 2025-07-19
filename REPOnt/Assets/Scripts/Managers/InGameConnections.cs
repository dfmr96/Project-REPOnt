using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System;

public class InGameConnections : MonoBehaviourPunCallbacks
{
    public static InGameConnections Instance { get; private set; }

    [SerializeField] private int maxReconnectAttempts = 3;
    [SerializeField] private float reconnectDelay = 5f;

    private int reconnectAttempts = 0;
    bool reconnected;
    private bool isTryingToReconnect = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 10000;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.ClientTimeout:
                UIManager.Instance.AddLogMessage("Client Timeout, intentando reconectar...", Color.cyan);
                StartCoroutine(TryToReconnect());
                break;
            case DisconnectCause.ExceptionOnConnect:
                UIManager.Instance.AddLogMessage("Error de red, intentando reconectar...", Color.yellow);
                StartCoroutine(TryToReconnect());
                break;

            case DisconnectCause.DisconnectByClientLogic:
                UIManager.Instance.AddLogMessage("Desconectando...", Color.yellow);
                SceneManager.LoadScene("MainMenu");
                break;

            case DisconnectCause.DisconnectByServerLogic:
                UIManager.Instance.AddLogMessage("El servidor te desconectó.", Color.yellow);
                SceneManager.LoadScene("MainMenu");
                break;

            case DisconnectCause.AuthenticationTicketExpired:
                UIManager.Instance.AddLogMessage("Sesi�n expirada.", Color.red);
                SceneManager.LoadScene("MainMenu");
                break;

            default:
                UIManager.Instance.AddLogMessage("Error desconocido, intentando reconectar...", Color.yellow);
                StartCoroutine(TryToReconnect());
                break;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.CustomProperties.TryGetValue("Role", out object role) && role.ToString() == "Ghost")
        {
            UIManager.Instance.AddLogMessage("El Ghost se desconectó. Esperando reconexión...", Color.red);
            Time.timeScale = 0f;
            StartCoroutine(WaitForGhostReconnect(10f));
        }
        else UIManager.Instance.AddLogMessage($"El jugador {otherPlayer.NickName} se desconectó.", Color.yellow);
    }

    private IEnumerator TryToReconnect()
    {
        if (isTryingToReconnect) yield break;
        isTryingToReconnect = true;

        reconnectAttempts = 0;

        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);

        while (reconnectAttempts < maxReconnectAttempts)
        {
            reconnectAttempts++;
            UIManager.Instance.AddLogMessage($"Intento de reconexión {reconnectAttempts} de {maxReconnectAttempts}",
                Color.yellow);

            if (PhotonNetwork.IsConnected)
            {
                yield return new WaitUntil(() =>
                    PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

                string lastRoomId = ConnectionManager.lastRoomId;
                bool rejoin = PhotonNetwork.RejoinRoom(lastRoomId);

                if (!rejoin)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
            }
            else
            {
                reconnected = PhotonNetwork.ReconnectAndRejoin();

                if (!reconnected)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
            }

            float timer = 0f;
            while (timer < reconnectDelay)
            {
                var state = PhotonNetwork.NetworkClientState;

                if (state == ClientState.Joined)
                {
                    UIManager.Instance.AddLogMessage("Reconectado con éxito.", Color.green);
                    isTryingToReconnect = false;
                    yield break;
                }

                if (state == ClientState.Disconnected) { break; }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        UIManager.Instance.AddLogMessage("Todos los intentos de reconexión fallaron.", Color.red);
        isTryingToReconnect = false;
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator WaitForGhostReconnect(float timeout)
    {
        for (float i = 0; i < timeout; i += Time.unscaledDeltaTime)
        {
            var ghostPlayer = Array.Find(PhotonNetwork.PlayerList, p =>
                p.CustomProperties.TryGetValue("Role", out object role) && role.ToString() == "Ghost");

            if (ghostPlayer != null)
            {
                UIManager.Instance.AddLogMessage("El Ghost reconectó con éxito.", Color.green);
                Time.timeScale = 1f;
                yield break;
            }

            yield return null;
        }

        UIManager.Instance.AddLogMessage("El Ghost no volvió a tiempo. Terminando la partida...", Color.red);
        Time.timeScale = 1f;
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
}