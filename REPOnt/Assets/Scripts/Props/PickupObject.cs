using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace Props
{
    public class PickupObject : MonoBehaviourPun
    {
        public void Interact(PhotonView playerPhotonView)
        {
            if (playerPhotonView != null)
            {
                photonView.RPC("RPC_HandlePickup", RpcTarget.AllBuffered, playerPhotonView.ViewID);
            }
        }

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
            
            if (mover.CurrentHandObject != null)
            {
                mover.CurrentHandObject.SetActive(false);
            }

            gameObject.SetActive(false);
            mover.Equip();
        }
    }
}