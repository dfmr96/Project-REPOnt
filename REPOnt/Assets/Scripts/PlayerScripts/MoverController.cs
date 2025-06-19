using Interfaces;
using Photon.Pun;
using Photon.Voice.Unity;
using Props;
using UnityEngine;

namespace PlayerScripts
{
    public class MoverController : PlayerBase
    {
        [Header("Interaction")]
        [SerializeField] private float interactRange = 3f;
        [SerializeField] GameObject currentHandObject;
        [Header("Player Settings")]
        [SerializeField] private KeyCode pushToTalkKey = KeyCode.J;
        private Recorder rec;
        [Header("Player Dependences")]
        [SerializeField] private GameObject micUIPrefab;
        [SerializeField] private AudioClip radioSound;
        private GameObject micUIInstance;
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
        }

        protected override void Update()
        {
            base.Update();
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
        
        public void PickupObject(PickupObject pickup)
        {
            ObjectId = pickup.PropID;
            if (CurrentHandObject != null)
            {
                CurrentHandObject.SetActive(true);
                //currentHandObjectRenderer.material.color = pickup.PropData.BaseColor;
            }
            
            Debug.Log($"[Mover] Picked up object with ID {ObjectId}");
        }

        public void ApplyWeightDebuff(float weight)
        {
            speedMultiplier = Mathf.Clamp(1f - (weight * .05f), .3f, 1f);
        }

        public void ResetSpeed() { speedMultiplier = 1f; }
        
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
            ResetSpeed();
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // UI logic
        // ──────────────────────────────────────────────────────────────────────────────
        private void ShowMicUI()
        {
            //Consulta para dave, capaz prefiere crear esto en el start para hacer una carga previa,
            //yo como capaz en micro no se usa prefiero que no exista hasta que no sea necesario
            //es un tema de tiempos en realidad porque en algun momento se tiene que crear jaja
            if (micUIInstance == null)
                micUIInstance = Instantiate(micUIPrefab);
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
