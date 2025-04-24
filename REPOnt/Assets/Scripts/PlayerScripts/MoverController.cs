using Photon.Pun;
using Props;
using UnityEngine;

namespace PlayerScripts
{
    public class MoverController : PlayerBase
    {
        [Header("Interaction")]
        [SerializeField] private float interactRange = 3f;
        [SerializeField] GameObject currentHandObject;
        
        
        private Renderer currentHandObjectRenderer;
        public GameObject CurrentHandObject => currentHandObject;
        public bool IsCaptured { get; private set; }
        public int ObjectId { get; set; }


        // ──────────────────────────────────────────────────────────────────────────────
        // Unity Methods
        // ──────────────────────────────────────────────────────────────────────────────
        
        protected override void Start()
        {
            base.Start();
            GameManager.Instance.RegisterMover(this);
            currentHandObjectRenderer = currentHandObject.GetComponentInChildren<Renderer>();
        }
        
        // ──────────────────────────────────────────────────────────────────────────────
        // Interaction Logic
        // ──────────────────────────────────────────────────────────────────────────────
        protected override void Interact()
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;
            
            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactRange))
            {
                Debug.DrawRay(origin, direction * interactRange, Color.blue, 1f);

                if (hit.collider.TryGetComponent(out PickupObject pickup))
                {
                    if (currentHandObject.activeInHierarchy) return;
                    
                    ObjectId = pickup.GetObjectId();
                    currentHandObjectRenderer.material.color = pickup.PropData.BaseColor;
                    pickup.Interact(photonView);
                    
                    Debug.Log("PickupObject Interacted");
                }
                else if (hit.collider.TryGetComponent(out DropZone dropZone))
                {
                    dropZone.Interact(photonView, ObjectId);
                    
                    Debug.Log("DropZone Interacted");
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * interactRange, Color.gray, 1f);
            }
        }
        
        // ──────────────────────────────────────────────────────────────────────────────
        // State Management
        // ──────────────────────────────────────────────────────────────────────────────
        public void Equip()
        {
            if (CurrentHandObject.activeSelf) return;
            CurrentHandObject.SetActive(true);
        }
        
        public void DropHandObject()
        {
            currentHandObjectRenderer.material.color = Color.cyan;
            CurrentHandObject.SetActive(false);
        }
        
        // ──────────────────────────────────────────────────────────────────────────────
        // RPC
        // ──────────────────────────────────────────────────────────────────────────────
        [PunRPC]
        public void MarkAsCaptured()
        {
            if (IsCaptured) return;

            IsCaptured = true;
            Debug.Log($"[MoverController] {photonView.Owner.NickName} has been captured.");
            GameManager.Instance.RegisterCapturedMover();

            // TODO Desactivar Inputs
        }
    }
}
