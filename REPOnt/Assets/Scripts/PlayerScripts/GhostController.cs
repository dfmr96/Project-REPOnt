using Photon.Pun;
using UnityEngine;

namespace PlayerScripts
{
    public class GhostController : PlayerBase
    {
        [Header("Interaction")] [SerializeField]
        private float interactionRange = 5f;

        [SerializeField] private float interactCooldown = 3f;
        [SerializeField] private Transform teleportTarget;

#if UNITY_EDITOR
        [Header("Debug Highlight")] [SerializeField]
        private Color debugHighlightColor = Color.yellow;

        private Renderer lastDebugRenderer;
        private Color lastOriginalColor;
#endif

        private float interactTimer = 0;

        protected override void Awake()
        {
            base.Awake();
            teleportTarget = GameManager.Instance.PrisonPoint;
        }

        protected override void Update()
        {
            base.Update();

            interactTimer += Time.deltaTime;

            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

#if UNITY_EDITOR
            DebugHighlightMover(origin, direction);
#endif
            if (Input.GetKeyDown(KeyCode.E) && interactTimer > interactCooldown) TryInteract(origin, direction);
        }

        private void TryInteract(Vector3 origin, Vector3 direction)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactionRange))
            {
                Debug.Log(
                    $"[TryInteract] Hit: {hit.collider.name}, Tag: {hit.collider.tag}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                if (!hit.collider.CompareTag("Mover")) return;

                if (hit.collider.TryGetComponent<PhotonView>(out PhotonView targetPV))
                {
                    Debug.Log($"[GhostInteraction] Hit mover: {targetPV.name}");
                    targetPV.RPC("TeleportToLocation", targetPV.Owner, teleportTarget.position);
                    interactTimer = 0f;
                }
            }
        }

#if UNITY_EDITOR
        private void DebugHighlightMover(Vector3 origin, Vector3 direction)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, interactionRange) &&
                hit.collider.CompareTag("Mover"))
            {
                if (hit.collider.TryGetComponent(out Renderer rend))
                {
                    if (rend != lastDebugRenderer)
                    {
                        ClearLastDebugHighlight();
                        lastDebugRenderer = rend;
                        lastOriginalColor = rend.material.color;
                    }

                    rend.material.color = debugHighlightColor;
                }
            }
            else ClearLastDebugHighlight();
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
