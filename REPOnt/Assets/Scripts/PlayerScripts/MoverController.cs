using Photon.Pun;
using Props;
using UnityEngine;

namespace PlayerScripts
{
    public class MoverController : PlayerBase
    {
        [Header("Interaction")]
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        [SerializeField] GameObject currentHandObject;

        public GameObject CurrentHandObject => currentHandObject;

        public void Equip()
        {
            if (CurrentHandObject.activeSelf) return;
        
            CurrentHandObject.SetActive(true);
        }

        public void DropHandObject()
        {
            CurrentHandObject.SetActive(false);
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(interactKey))
            {
                TryInteract();
            }
        }

        private void TryInteract()
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactRange))
            {
                Debug.DrawRay(origin, direction * interactRange, Color.blue, 1f);

                if (hit.collider.TryGetComponent(out PickupObject pickup))
                {
                    pickup.Interact(photonView);
                    Debug.Log("PickupObject Interacted");
                }
                else if (hit.collider.TryGetComponent(out DropZone dropZone))
                {
                    dropZone.Interact(photonView);
                    Debug.Log("DropZone Interacted");
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * interactRange, Color.gray, 1f);
            }
        }
    }
}
