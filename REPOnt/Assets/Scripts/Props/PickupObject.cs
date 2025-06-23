using System;
using Interfaces;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class PickupObject : PropBehaviourBase, IInteractable
    {
        private Vector3 rotationVel = new Vector3(0, 25, 0);
        private void Update() { transform.Rotate(rotationVel * Time.deltaTime); }
        protected override Color GetAssignedColor() => propData.BaseColor;
        public void Interact(PhotonView actorView, int _ = -1)
        {
            photonView.RPC(nameof(RPC_HandlePickup), RpcTarget.AllBuffered, actorView.ViewID);
        }
        public void Drop(PhotonView actorView)
        {
            photonView.RPC(nameof(RPC_HandleDrop), RpcTarget.AllBuffered, actorView.ViewID);
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

            // Lógica que TODOS deben ver
            mover.ObjectId = propData.ID;
            mover.ShowHandObject();

            // Lógica que SOLO el dueño ve (UI)
            if (mover.GetComponent<PhotonView>().IsMine)
            {
                mover.ShowObjectUI(propData.PropPreview);
            }

            mover.ApplyWeightDebuff(propData.Weight);
            mover.GetPickUpObject(this);
            gameObject.SetActive(false);
        }

        [PunRPC]
        public void RPC_HandleDrop(int playerViewID)
        {
            var mover = GameManager.Instance.GetMoverByViewID(playerViewID);
            if (mover == null) return;
            if (mover.GetComponent<PhotonView>().IsMine) { mover.HideObjectUI(); }
            mover.DropHandObject();
            mover.ResetSpeed();
            gameObject.SetActive(true);
            gameObject.transform.position = mover.transform.position;
        }
    }
}