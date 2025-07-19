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
            if (interactor.TryGetComponent(out MoverController mover))
            {
                if (mover.IsCaptured) return;

                if (gateController.IsBusy) return;

                gateController.OpenGate();
            }
        }
    }
}