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
        [SerializeField] private Light spotlight;
        protected override Color GetAssignedColor() => propData.DropZoneColor;
 
        public void Interact(PhotonView playerPhotonView, int objectId)
        {
            if (isPlaced || objectId != propData.ID) return;
            photonView.RPC(nameof(RPC_PlaceObject), RpcTarget.AllBuffered, playerPhotonView.ViewID);
        }

        public override void SetPropData(PropData data)
        {
            base.SetPropData(data);
            MaterialUtils.SetGrayAlpha(newProp, 0.4f);
        }

        [PunRPC]
        private void RPC_PlaceObject(int playerViewID)
        {
            MoverController mover = GameManager.Instance.GetMoverByViewID(playerViewID);
            if (mover == null) return;

            spotlight.intensity = 1f;
            spotlight.range *= 2f;
            MaterialUtils.RestoreOriginalMaterials(newProp);
            mover.DropHandObject();
            isPlaced = true;
            //rend.material.color = placedColor;
            GameManager.Instance.RegisterPropPlaced();
            GameAnalyticsHandler.TrackObjectPlaced(playerViewID);
        }
    }
}