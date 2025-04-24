using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

namespace PlayerScripts
{
    public class PlayerSpawner : MonoBehaviourPunCallbacks
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform[] moverSpawnPoints;
        [SerializeField] private Transform ghostSpawnPoint;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject moverPrefab;
        [SerializeField] private GameObject ghostPrefab;
        
        private const string RoleKey = "Role";
        private const string GhostRole = "Ghost";
        private const string MoverRole = "Mover";

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            SpawnLocalPlayer();
        }

        void SpawnLocalPlayer()
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object roleObj))
            {
                Debug.LogWarning("No role found for player.");
                return;
            }

            string role = roleObj.ToString();
            
            switch (role)
            {
                case GhostRole:
                    SpawnGhost();
                    break;
                case MoverRole:
                    SpawnMover();
                    break;
                default:
                    Debug.LogWarning($"Unknown role: {role}");
                    break;
            }
        }
        
        private void SpawnGhost()
        {
            PhotonNetwork.Instantiate(ghostPrefab.name, ghostSpawnPoint.position, Quaternion.identity);
        }
        
        private void SpawnMover()
        {
            int index = GetMoverSpawnIndexByOrder(PhotonNetwork.LocalPlayer);
            index = Mathf.Clamp(index, 0, moverSpawnPoints.Length - 1);

            Vector3 spawnPosition = moverSpawnPoints[index].position;
            PhotonNetwork.Instantiate(moverPrefab.name, spawnPosition, Quaternion.identity);
        }

        private int GetMoverSpawnIndexByOrder(Player localPlayer)
        {
            int index = 0;

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.ActorNumber == localPlayer.ActorNumber)
                    break;

                if (player.CustomProperties.TryGetValue(RoleKey, out object role) && role.ToString() == MoverRole)
                    index++;
            }

            return index;
        }
    }
}
