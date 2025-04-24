using System;
using Interfaces;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class PickupObject : PropBehaviourBase, IInteractable
    {
        protected override Color GetAssignedColor() => propData.BaseColor;
        public void Interact(PhotonView actorView, int _ = -1)
        {
            photonView.RPC(nameof(RPC_HandlePickup), RpcTarget.AllBuffered, actorView.ViewID);
        }
        
        [PunRPC]
        public void RPC_HandlePickup(int playerViewID)
        {
            var mover = GameManager.Instance.GetMoverByViewID(playerViewID);
            if (mover == null)
            {
                Debug.LogWarning("MoverController not found on player");
                return;
            }
            
            mover.PickupObject(this);

            gameObject.SetActive(false);
        }
    }
}