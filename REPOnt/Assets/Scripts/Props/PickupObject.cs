using System;
using Interfaces;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class PickupObject : MonoBehaviourPun, IInteractable
    {
        [SerializeField] private PropData propData;
        [SerializeField] private Renderer rend;
        public PropData PropData => propData;
        public int PropID => propData.ID;

        private void Start()
        {
            if (rend == null)
                rend = GetComponent<Renderer>();
        }
        
        public void Interact(PhotonView actorView, int _ = -1)
        {
            photonView.RPC(nameof(RPC_HandlePickup), RpcTarget.AllBuffered, actorView.ViewID);
        }
        
        
        public void SetPropData(PropData data)
        {
            propData = data;
            if (rend != null && data != null)
            {
                rend.material.color = data.BaseColor;
            }
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