using Interfaces;
using Photon.Pun;
using Photon.Voice.Unity;
using Props;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerScripts
{
    public class MoverController : PlayerBase
    {
        [Header("Interaction")] [SerializeField]
        private float interactRange = 3f;

        [SerializeField] GameObject currentHandObject;

        [Header("Player Settings")] [SerializeField]
        private KeyCode pushToTalkKey = KeyCode.J;
        private KeyCode dropObjectKey = KeyCode.Q;

        private Recorder rec;

        [Header("Player Dependences")] [SerializeField]
        private GameObject micUIPrefab;

        [SerializeField] private GameObject objUIPrefab;
        [SerializeField] private GameObject playerCanvasPrefab;
        [SerializeField] private AudioClip radioSound;
        private GameObject playerCanvas;
        private GameObject micUIInstance;
        private GameObject objUIInstance;
        private AudioSource audioSource;
        private PickupObject pickupObject = null;

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
            rec = GetComponent<Recorder>();
            audioSource = GetComponent<AudioSource>();
            if (playerCanvas == null) playerCanvas = Instantiate(playerCanvasPrefab);
        }

        protected override void Update()
        {
            base.Update();
            if (!photonView.IsMine) return;
            if (Input.GetKeyDown(pushToTalkKey))
            {
                rec.TransmitEnabled = true;
                ShowMicUI();
                PlayRadioSound();
            }
            else if (Input.GetKeyUp(pushToTalkKey))
            {
                rec.TransmitEnabled = false;
                HideMicUI();
            }
            if (Input.GetKeyDown(dropObjectKey))
            {
                if (pickupObject != null)
                {
                    pickupObject.Drop(photonView);
                    photonView.RPC(nameof(RPC_HandleDropObject), RpcTarget.All, pickupObject.PropID);
                    pickupObject = null;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Prison"))
            {
                if (!IsCaptured) return;
                IsCaptured = false;
                GameManager.Instance.UpdateMoversCaptured();
                if (!photonView.IsMine) return;
                photonView.RPC(nameof(RPC_UnmarkAsCaptured), RpcTarget.All);
            }
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
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                    interactable.Interact(photonView, ObjectId);
            }
        }

        public void ShowHandObject()
        {
            if (CurrentHandObject != null)
                CurrentHandObject.SetActive(true);
        }

        public void ShowObjectUI(Sprite objImage)
        {
            if (playerCanvas == null) return;

            if (objUIInstance == null)
                objUIInstance = Instantiate(objUIPrefab, playerCanvas.transform);

            objUIInstance.GetComponent<Image>().sprite = objImage;
            objUIInstance.SetActive(true);
        }

        public void HideObjectUI()
        {
            if (playerCanvas == null) return;
            objUIInstance.SetActive(false);
        }

        public void ApplyWeightDebuff(float weight) { speedMultiplier = Mathf.Clamp(1f - (weight * .05f), .3f, 1f); }

        public void ResetSpeed() { speedMultiplier = 1f; }

        public void GetPickUpObject(PickupObject obj) { pickupObject = obj; }

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
            ObjectId = -1;
            ResetSpeed();
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // UI logic
        // ──────────────────────────────────────────────────────────────────────────────
        private void ShowMicUI()
        {
            if (micUIInstance == null)
                micUIInstance = Instantiate(micUIPrefab, playerCanvas.transform);
            micUIInstance.SetActive(true);
        }

        private void HideMicUI()
        {
            if (micUIInstance != null)
                micUIInstance.SetActive(false);
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // Audio Logic
        // ──────────────────────────────────────────────────────────────────────────────
        private void PlayRadioSound() { audioSource.PlayOneShot(radioSound); }

        // ──────────────────────────────────────────────────────────────────────────────
        // RPC
        // ──────────────────────────────────────────────────────────────────────────────
        [PunRPC]
        public void MarkAsCaptured()
        {
            if (IsCaptured) return;
            IsCaptured = true;
            if (pickupObject != null) 
            {
                pickupObject.ReturnObject(photonView);
                if (PlayerRoleHelper.IsLocalPlayerGhost()) 
                    photonView.RPC(nameof(RPC_HandleDropObject), RpcTarget.All, pickupObject.PropID);
                pickupObject = null;
            }
            GameManager.Instance.RegisterCapturedMover();
            if (!PlayerRoleHelper.IsLocalPlayerGhost()) return;
            GameAnalyticsHandler.TrackCapturedPlayers(photonView.Owner.ActorNumber, PhotonNetwork.CurrentRoom.Name);
        }

        [PunRPC]
        public void RPC_UnmarkAsCaptured()
        {
            if (!PlayerRoleHelper.IsLocalPlayerGhost()) return;
            GameAnalyticsHandler.TrackUnCapturedPlayers(photonView.Owner.ActorNumber, PhotonNetwork.CurrentRoom.Name);
        }

        [PunRPC]
        public void RPC_HandleDropObject(int objId)
        {
            if (!PlayerRoleHelper.IsLocalPlayerGhost()) return;
            GameAnalyticsHandler.TrackObjectDropped(photonView.Owner.ActorNumber, objId, PhotonNetwork.CurrentRoom.Name);
        }
    }
}