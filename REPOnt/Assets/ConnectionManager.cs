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

    public override void OnConnectedToMaster() { OnConnectToMaster.Invoke(); }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.CreateRoom(GenerateId());
    }

    public override void OnCreatedRoom() 
    { 
        OnRoomCreated.Invoke();
        OnRoomJoined.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public string GetRunId() {  return PhotonNetwork.CurrentRoom.Name; }

    public void JoinRoom(string roomId)
    {
        if (!PhotonNetwork.IsConnected) return;

        Debug.Log("Intentando unirse a sala: " + roomId);
        OnRoomJoined.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.JoinRoom(roomId);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) { OnRoomJoinUpdated.Invoke(message); }

    public override void OnJoinedRoom()
    {
        OnRoomJoinUpdated?.Invoke("");
        PhotonNetwork.NickName = "Player " + PhotonNetwork.PlayerList.Length;
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 5) PhotonNetwork.LoadLevel(sceneToLoad);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) { OnRoomJoined.Invoke(PhotonNetwork.CurrentRoom.PlayerCount); }

    public override void OnPlayerLeftRoom(Player otherPlayer) { OnRoomJoined.Invoke(PhotonNetwork.CurrentRoom.PlayerCount); }

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
}
