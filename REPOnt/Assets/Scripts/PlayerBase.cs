using System;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private PlayerData data;
    [SerializeField] private Camera playerCamera;
    protected PhotonView photonView;
    private Vector3 rotation = Vector3.zero;
    private float cameraAngle;
    protected virtual void Awake() { photonView = GetComponent<PhotonView>(); }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            playerCamera.enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (!photonView.IsMine) return;
        Move();
        Look();
    }

    protected virtual void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        transform.Translate(move * (Time.deltaTime * data.NormalSpeed));
    }

    protected virtual void Look()
    {
        //5f va a ser por ahora la sensibilidad pero esto deberia cambiar para cada jugador
        rotation.x = Input.GetAxis("Mouse X") * Time.deltaTime * 100f;
        rotation.y = Input.GetAxis("Mouse Y") * Time.deltaTime * 100f;

        cameraAngle += rotation.y;
        cameraAngle = Mathf.Clamp(cameraAngle, -70, 70);

        transform.Rotate(Vector3.up * rotation.x);
        playerCamera.transform.localRotation = Quaternion.Euler(-cameraAngle, 0, 0);
    }

    [PunRPC]
    public virtual void TeleportToLocation(Vector3 newPosition) { transform.position = newPosition; }
}
