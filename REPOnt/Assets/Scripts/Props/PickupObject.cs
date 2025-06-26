using System;
using Interfaces;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class PickupObject : PropBehaviourBase, IInteractable
    {
        [SerializeField] private LayerMask groundLayer;
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
        public void ReturnObject(PhotonView actorView)
        {
            photonView.RPC(nameof(RPC_BackToOrigin), RpcTarget.AllBuffered, actorView.ViewID);
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

            Vector3 dropOrigin = mover.transform.position + (mover.transform.forward * 2f) + Vector3.up * 2f;
            Vector3 dropDirection = Vector3.down;

            Debug.DrawRay(dropOrigin, dropDirection * 20f, Color.red, 5f);

            if (Physics.Raycast(dropOrigin, dropDirection, out RaycastHit hit, 20f))
                gameObject.transform.position = hit.point;
            else gameObject.transform.position = dropOrigin + Vector3.down * 2f;
        }

        [PunRPC]
        public void RPC_BackToOrigin(int playerViewID)
        {
            var mover = GameManager.Instance.GetMoverByViewID(playerViewID);
            if (mover == null) return;
            if (mover.GetComponent<PhotonView>().IsMine) { mover.HideObjectUI(); }
            mover.DropHandObject();
            mover.ResetSpeed();
            gameObject.SetActive(true);
        }
    }
}