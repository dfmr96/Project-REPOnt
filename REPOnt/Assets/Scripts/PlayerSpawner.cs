using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Transform[] moverSpawnPoints;
    public Transform ghostSpawnPoint;

    public GameObject moverPrefab;
    public GameObject ghostPrefab;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SpawnPlayerByRole();
    }

    void SpawnPlayerByRole()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object roleObj))
        {
            Debug.LogWarning("No role found for player.");
            Debug.LogWarning($"{PhotonNetwork.IsMasterClient} {PhotonNetwork.CurrentRoom.PlayerCount}");;
            return;
        }

        string role = roleObj.ToString();

        if (role == "Ghost")
        {
            PhotonNetwork.Instantiate(ghostPrefab.name, ghostSpawnPoint.position, Quaternion.identity);
        }
        else if (role == "Mover")
        {
            int spawnIndex = GetMoverSpawnIndex(PhotonNetwork.LocalPlayer);
            spawnIndex = Mathf.Clamp(spawnIndex, 0, moverSpawnPoints.Length - 1); 

            Vector3 spawnPos = moverSpawnPoints[spawnIndex].position;
            PhotonNetwork.Instantiate(moverPrefab.name, spawnPos, Quaternion.identity);
        }
    }

    int GetMoverSpawnIndex(Player player)
    {
        int index = 0;

        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == player.ActorNumber)
                break;

            if (p.CustomProperties.TryGetValue("Role", out object r) && r.ToString() == "Mover")
                index++;
        }

        return index;
    }
}
