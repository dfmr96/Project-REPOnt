using Photon.Pun;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PlayerScripts
{
    public class GhostController : PlayerBase
    {
        [Header("Interaction")] 
        [SerializeField] private float interactionRange = 5f;
        [SerializeField] private Transform teleportTarget;

        [Header("Analytics Tracker")]
        [SerializeField] private float afkDistanceThreshold = 1f;
        [SerializeField] private float afkTimeThreshold = 10f;
        private Vector3 lastPosition;
        private float afkTimer = 0f;

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
            TrackAFKGhost();

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
                    targetPV.RPC("MarkAsCaptured", RpcTarget.AllBuffered);
                    targetPV.RPC(nameof(TeleportToLocation), targetPV.Owner, teleportTarget.position);

                    //photonView.RPC(nameof(RPC_HandleMoverCapture), RpcTarget.All, targetPV);
                }
            }
        }
        private void TrackAFKGhost()
        {
            float movedDistance = Vector3.Distance(transform.position, lastPosition);

            if (movedDistance <= afkDistanceThreshold)
            {
                afkTimer += Time.deltaTime;

                if (afkTimer >= afkTimeThreshold)
                {
                    photonView.RPC(nameof(RPC_HandleGhostAFK), RpcTarget.MasterClient);
                    afkTimer = 0f;
                }
            }
            else
            {
                afkTimer = 0f;
                lastPosition = transform.position;
            }
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // RPC
        // ──────────────────────────────────────────────────────────────────────────────
        [PunRPC]
        private void RPC_HandleGhostAFK()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            GameAnalyticsHandler.TrackGhostAFK(afkTimer, transform.position, PhotonNetwork.CurrentRoom.Name);
        }

        //[PunRPC]
        //private void RPC_HandleMoverCapture(PhotonView targetPV)
        //{
        //    if (!PhotonNetwork.IsMasterClient) return;
        //    GameAnalyticsHandler.TrackCapturedPlayers(targetPV.Owner.ActorNumber, PhotonNetwork.CurrentRoom.Name);
        //}


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
