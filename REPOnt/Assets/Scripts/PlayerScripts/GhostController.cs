using Photon.Pun;
using UnityEngine;

namespace PlayerScripts
{
    public class GhostController : PlayerBase
    {
        [Header("Interaction")] 
        [SerializeField] private float interactionRange = 5f;
        [SerializeField] private Transform teleportTarget;
        
#if UNITY_EDITOR
        [Header("Debug Highlight")] 
        [SerializeField] private Color debugHighlightColor = Color.yellow;
        private Renderer lastDebugRenderer;
        private Color lastOriginalColor;
#endif

        // ──────────────────────────────────────────────────────────────────────────────
        // Unity Methods
        // ──────────────────────────────────────────────────────────────────────────────

        protected override void Awake()
        {
            base.Awake();
            teleportTarget = GameManager.Instance.PrisonPoint;
        }

        protected override void Update()
        {
            if (!photonView.IsMine) return;

#if UNITY_EDITOR
            DebugHighlightMover(transform.position, transform.forward);
#endif
            base.Update();
        }
        
        // ──────────────────────────────────────────────────────────────────────────────
        // Character Logic
        // ──────────────────────────────────────────────────────────────────────────────

        protected override void Interact()
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactionRange))
            {
                if (hit.collider.CompareTag("Mover") && hit.collider.TryGetComponent(out PhotonView targetPV))
                {
                    Debug.Log($"[Ghost] Capturing mover: {targetPV.name}");
                    targetPV.RPC("TeleportToLocation", targetPV.Owner, teleportTarget.position);
                    targetPV.RPC("MarkAsCaptured", RpcTarget.AllBuffered);
                }
            }
        }
        
        // ──────────────────────────────────────────────────────────────────────────────
        // Debug methods
        // ──────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR
        private void DebugHighlightMover(Vector3 origin, Vector3 direction)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactionRange) &&
                hit.collider.CompareTag("Mover") &&
                hit.collider.TryGetComponent(out Renderer rend))
            {
                if (rend != lastDebugRenderer)
                {
                    ClearLastDebugHighlight();
                    lastDebugRenderer = rend;
                    lastOriginalColor = rend.material.color;
                }

                rend.material.color = debugHighlightColor;
            }
            else
            {
                ClearLastDebugHighlight();
            }
        }

        private void ClearLastDebugHighlight()
        {
            if (lastDebugRenderer != null)
            {
                lastDebugRenderer.material.color = lastOriginalColor;
                lastDebugRenderer = null;
            }
        }
#endif
        
        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position;
            Vector3 dir = transform.forward * interactionRange;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin, dir);
            Gizmos.DrawWireSphere(origin + dir, 0.1f);
        }
    }
}
