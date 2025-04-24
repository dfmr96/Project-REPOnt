using System;
using Interfaces;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class DropZone : PropBehaviourBase, IInteractable
    {
        [SerializeField] private Color placedColor = Color.green;
        private bool isPlaced = false;
        public bool IsPlaced => isPlaced;
        protected override Color GetAssignedColor() => propData.DropZoneColor;
 
        public void Interact(PhotonView playerPhotonView, int objectId)
        {
            if (isPlaced || objectId != propData.ID) return;
            photonView.RPC(nameof(RPC_PlaceObject), RpcTarget.AllBuffered, playerPhotonView.ViewID);
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