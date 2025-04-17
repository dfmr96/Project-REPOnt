using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System;
using Photon.Realtime;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public static ConnectionManager Instance;
    public Action OnConnectToMaster;
    public Action OnRoomCreated;
    public Action<int> OnRoomJoined;
    public Action<string> OnRoomJoinUpdated;
    [SerializeField] private int sceneToLoad = 1;
    private const int MinPlayersRequired = 3;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start() { ConnectToPhoton(); }

    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        OnConnectToMaster?.Invoke();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.CreateRoom(GenerateId());
    }

    public override void OnCreatedRoom() 
    { 
        OnRoomCreated?.Invoke();
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public string GetRunId() {  return PhotonNetwork.CurrentRoom.Name; }

    public void JoinRoom(string roomId)
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.JoinRoom(roomId);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        OnRoomJoinUpdated.Invoke(message);
    }

    public override void OnJoinedRoom()
    {
        OnRoomJoinUpdated?.Invoke("Joined");
        PhotonNetwork.NickName = "Player " + PhotonNetwork.PlayerList.Length;
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
        //if (PhotonNetwork.CurrentRoom.PlayerCount >= 2) PhotonNetwork.LoadLevel(sceneToLoad);
        //El LoadLevel manda a todos a cargar la escena, solo lo debe hacer el master client cuando se cumpla el minimo de jugadores
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount >= MinPlayersRequired && PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
            
            StartCoroutine(WaitBeforeStartingGame()); 
            //Setear los roles es asincrono y entra en race condition usando directamente LoadLevel
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public int GetPlayersQuantity() { return PhotonNetwork.PlayerList.Length; }

    private string GenerateId()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string id = "";

        for (int i = 0; i < 6; i++)
        {
            int index = UnityEngine.Random.Range(0, chars.Length);
            id += chars[index];
        }
        return id;
    }

    public void AssignRoles(int minPlayersRequired = MinPlayersRequired)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var players = PhotonNetwork.PlayerList;
        if (players.Length < minPlayersRequired) return;

        int ghostIndex = 0;
        
#if UNITY_EDITOR
    // El Ghost será siempre el MasterClient (jugador que crea la sala)
        Player master = PhotonNetwork.MasterClient;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == master)
            {
                ghostIndex = i;
                break;
            }
        }
#else
        //En builds normales: Ghost aleatorio
        ghostIndex = UnityEngine.Random.Range(0, players.Length);
#endif

        for (int i = 0; i < players.Length; i++)
        {
            string role = (i == ghostIndex) ? "Ghost" : "Mover";
            string host = (PhotonNetwork.PlayerList[i].IsMasterClient) ? "Host" : "Guest";
            string nickname = $"Player {i + 1}, ({host})";

            ExitGames.Client.Photon.Hashtable roleProp = new ExitGames.Client.Photon.Hashtable();
            roleProp["Role"] = role;

            players[i].SetCustomProperties(roleProp);
            
            Debug.Log($"[AssignRoles] {nickname} assigned role: {role}");
        }
    }
    
    private IEnumerator WaitBeforeStartingGame()
    {
        AssignRoles();
        
        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.LoadLevel(sceneToLoad);
    }
}
