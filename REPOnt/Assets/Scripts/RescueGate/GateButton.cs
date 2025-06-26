using System;
using Interfaces;
using Photon.Pun;
using PlayerScripts;
using UnityEngine;

namespace RescueGate
{
    public class GateButton : MonoBehaviour, IInteractable
    {
        [SerializeField] private GateController gateController;

        private void Start()
        {
            gateController.OnGateOpened += () => Debug.Log("Gate opened!");
            gateController.OnGateClosed += () => Debug.Log("Gate closed!");
        }

        public void Interact(PhotonView interactor, int objectId)
        {
            Debug.Log("[GateButton] Interact called");

            if (interactor.TryGetComponent(out MoverController mover))
            {
                if (mover.IsCaptured)
                {
                    Debug.Log("[GateButton] Mover is captured, can't open.");
                    return;
                }

                if (gateController.IsBusy)
                {
                    Debug.Log("[GateButton] Gate is currently moving or already open.");
                    return;
                }

                Debug.Log("[GateButton] Valid mover, opening gate.");
                gateController.OpenGate();
            }
            else
            {
                Debug.Log("[GateButton] Interactor is not a Mover.");
            }
        }
    }
}