using Interfaces;
using Photon.Pun;
using Photon.Voice.Unity;
using Props;
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
        private KeyCode test = KeyCode.U;

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
            if (playerCanvas == null)
            {
                playerCanvas = Instantiate(playerCanvasPrefab);
            }
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

                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(photonView, ObjectId);
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * interactRange, Color.gray, 1f);
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

        //Se quito el uso de este moetod por ambos de arriba TODO Cambiar
        public void PickupObject(PickupObject pickup, Sprite objImage, Color objColor)
        {
            ObjectId = pickup.PropID;
            if (CurrentHandObject != null)
            {
                CurrentHandObject.SetActive(true);
                //if (objUIInstance == null) { Instantiate(objUIPrefab, playerCanvas.transform); }
                //objUIInstance.GetComponent<Image>().sprite = objImage;
                //objUIInstance.GetComponent<Image>().color = objColor;
                //objUIInstance.SetActive(true);
                //currentHandObjectRenderer.material.color = pickup.PropData.BaseColor;
            }

            Debug.Log($"[Mover] Picked up object with ID {ObjectId}");
        }

        public void ApplyWeightDebuff(float weight)
        {
            speedMultiplier = Mathf.Clamp(1f - (weight * .05f), .3f, 1f);
        }

        public void ResetSpeed()
        {
            speedMultiplier = 1f;
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
            //objUIInstance.SetActive(false);
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
        private void PlayRadioSound()
        {
            //Aca deberia llamar al audiosource y reproducirle un sonidito asi re loco jajant
            audioSource.PlayOneShot(radioSound);
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