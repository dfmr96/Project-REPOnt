using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public static ConnectionManager Instance { get; private set; }
    [SerializeField] private int sceneToLoad = 1;
    [SerializeField] private int minPlayersRequired = 3;
    private const string RoleKey = "Role";
    private const string GhostRole = "Ghost";
    private const string MoverRole = "Mover";
    
    public Action OnConnectToMaster;
    public Action OnRoomCreated;
    public Action<int> OnRoomJoined;
    public Action<string> OnRoomJoinUpdated;
    public Action OnHostLeftRoom;
    // ──────────────────────────────────────────────────────────────────────────────
    // Unity Methods
    // ──────────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start() { ConnectToPhoton(); }
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Pun Callbacks
    // ──────────────────────────────────────────────────────────────────────────────

    public override void OnConnectedToMaster()
    {
        OnConnectToMaster?.Invoke();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnCreatedRoom() 
    { 
        OnRoomCreated?.Invoke();
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinedRoom()
    {
        OnRoomJoinUpdated?.Invoke("Joined");
        PhotonNetwork.NickName = "Player " + PhotonNetwork.PlayerList.Length;
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinRoomFailed(short returnCode, string message) { OnRoomJoinUpdated.Invoke(message); }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
        /*if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersRequired && PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
            
            StartCoroutine(WaitBeforeStartingGame()); 
            //Setear los roles es asincrono y entra en race condition usando directamente LoadLevel
        }*/
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnRoomJoined?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────────────────────────────────────
    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.CreateRoom(GenerateId());
    }

    public void JoinRoom(string roomId)
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.JoinRoom(roomId);
    }
    public string GetRoomId() {  return PhotonNetwork.CurrentRoom.Name; }
    public int GetPlayersQuantity() { return PhotonNetwork.PlayerList.Length; }

    public bool CanStartGame() => PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersRequired;
    
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
    public void JoinRandomRoom()
    {
        if (!PhotonNetwork.IsConnected) return;

        PhotonNetwork.JoinRandomRoom();
    }
    
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient || !CanStartGame()) return;

        AssignRoles();
        StartCoroutine(WaitBeforeStartingGame());
    }
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Role Assignment
    // ──────────────────────────────────────────────────────────────────────────────

    public void AssignRoles()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var players = PhotonNetwork.PlayerList;
        if (players.Length < minPlayersRequired) return;

        int ghostIndex = GetGhostIndex(players);

        for (int i = 0; i < players.Length; i++)
        {
            string role = (i == ghostIndex) ? GhostRole : MoverRole;
            string host = (PhotonNetwork.PlayerList[i].IsMasterClient) ? "Host" : "Guest";
            string nickname = $"Player {i + 1}, ({host})";

            var roleProp = new ExitGames.Client.Photon.Hashtable
            {
                [RoleKey] = role
            };

            players[i].SetCustomProperties(roleProp);
            
            Debug.Log($"[AssignRoles] {nickname} assigned role: {role}");
        }
    }
    
    private int GetGhostIndex(Player[] players)
    {
#if UNITY_EDITOR
        return Array.FindIndex(players, p => p == PhotonNetwork.MasterClient);
#else
    return UnityEngine.Random.Range(0, players.Length);
#endif
    }
    
    private IEnumerator WaitBeforeStartingGame()
    {
        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.LoadLevel(sceneToLoad);
    }
    
    public void LeaveRoomAsHost()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ForceClientsToLeave), RpcTarget.Others);
        }

        PhotonNetwork.LeaveRoom();
    }
    
    [PunRPC]
    public void RPC_ForceClientsToLeave()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[ConnectionManager] Host has left the room. Disconnecting...");
            OnHostLeftRoom?.Invoke();
        }
    }
    
}
