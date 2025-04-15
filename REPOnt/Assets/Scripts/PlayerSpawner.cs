using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject prefab;

    void Start() { PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity); }
}
