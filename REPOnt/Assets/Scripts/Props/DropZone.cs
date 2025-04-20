using System;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class DropZone : MonoBehaviourPun
    {
        [SerializeField] private Color placedColor = Color.green;
        [SerializeField] private ObjectData data;
        private bool isPlaced = false;

        public bool IsPlaced => isPlaced;

        private void Start()
        {
            GameManager.Instance.RegisterDropZone(this);
        }

        public void Interact(PhotonView playerPhotonView, int objectId)
        {
            if (isPlaced) return;
            if (objectId != data.id) return;
            photonView.RPC(nameof(RPC_PlaceObject), RpcTarget.AllBuffered, playerPhotonView.ViewID);
        }

        [PunRPC]
        private void RPC_PlaceObject(int playerViewID)
        {
            GameObject player = PhotonView.Find(playerViewID).gameObject;
            MoverController mover = player.GetComponent<MoverController>();
            if (mover == null) return;

            // Cambia color del cubo
            GetComponent<Renderer>().material.color = placedColor;

            // Elimina el objeto de la mano
            mover.DropHandObject();

            isPlaced = true;
            GameManager.Instance.CheckDropCompletion();
        }
    }
}