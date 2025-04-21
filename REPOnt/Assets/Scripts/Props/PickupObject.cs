using System;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class PickupObject : MonoBehaviourPun
    {
        [SerializeField] private PropData propData;
        [SerializeField] private Renderer rend;

        private void Start()
        {
            rend = GetComponent<Renderer>();
            //The assignment is done in the PropAssignmentManager
            /*if (propData != null && rend != null)
            {
                rend.material.color = propData.BaseColor;
            }*/
        }

        public void Interact(PhotonView playerPhotonView)
        {
            if (playerPhotonView != null) photonView.RPC(nameof(RPC_HandlePickup), RpcTarget.AllBuffered, playerPhotonView.ViewID);
        }

        public int GetObjectId() { return propData.ID; }

        [PunRPC]
        public void RPC_HandlePickup(int playerViewID)
        {
            //TODO Intentar pasar player a través del RPC para no usar Find (Si no funciona, es que los tipo de parametro validos de los RPC son limitados)
            //TODO Utilizar TryGetComponent para ahorrar if's
            //TODO Borrar debugs cuando lo de arriba funcione
            GameObject player = PhotonView.Find(playerViewID)?.gameObject;

            if (player == null)
            {
                Debug.LogWarning("Player GameObject not found via PhotonView ID");
                return;
            }
            //TODO Sacar el texto suelto de mover y llevarlo a un metodo en MoverController
            MoverController mover = player.GetComponent<MoverController>();
            if (mover == null)
            {
                Debug.LogWarning("MoverController not found on player");
                return;
            }
            
            if (mover.CurrentHandObject != null) { mover.CurrentHandObject.SetActive(false); }

            gameObject.SetActive(false);
            mover.Equip();
        }
        
        public void SetPropData(PropData data)
        {
            propData = data;

            if (rend != null && propData != null)
            {
                rend.material.color = propData.BaseColor;
            }
        }
        //TODO DropZone y PickObject tienen varios metodos en comun. Se podrian unificar en una clase base
    }
}