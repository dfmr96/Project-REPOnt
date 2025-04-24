using System;
using Interfaces;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class DropZone : MonoBehaviourPun, IInteractable
    {
        [SerializeField] private Color placedColor = Color.green;
        [SerializeField] private PropData propData;
        [SerializeField] private Renderer rend;
        private bool isPlaced = false;

        public bool IsPlaced => isPlaced;

        private void Start()
        {
            rend = GetComponent<Renderer>();
            
            // The assignment is done in the PropAssignmentManager
            /*if (propData != null && rend != null)
            {
                rend.material.color = propData.DropZoneColor;
            }*/
        }

        public void Interact(PhotonView playerPhotonView, int objectId)
        {
            if (isPlaced) return;
            if (objectId != propData.ID) return;
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
            GameManager.Instance.RegisterPropPlaced();
        }
        
        public void SetPropData(PropData data)
        {
            propData = data;

            if (rend != null && propData != null)
            {
                rend.material.color = propData.DropZoneColor;
            }
        }
    }
}