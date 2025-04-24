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
            if (rend == null)
                rend = GetComponent<Renderer>();
        }

        public void Interact(PhotonView playerPhotonView, int objectId)
        {
            if (isPlaced || objectId != propData.ID) return;

            photonView.RPC(nameof(RPC_PlaceObject), RpcTarget.AllBuffered, playerPhotonView.ViewID);
        }
        public void SetPropData(PropData data)
        {
            propData = data;
            isPlaced = false;

            if (rend != null && data != null)
                rend.material.color = data.DropZoneColor;
        }

        [PunRPC]
        private void RPC_PlaceObject(int playerViewID)
        {
            MoverController mover = GameManager.Instance.GetMoverByViewID(playerViewID);
            if (mover == null) return;

            mover.DropHandObject();
            isPlaced = true;
            rend.material.color = placedColor;
            GameManager.Instance.RegisterPropPlaced();
        }
        
    }
}