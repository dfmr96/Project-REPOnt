using Photon.Pun;
using PlayerScripts;
using UnityEngine;
[RequireComponent(typeof(PhotonView))]
public class PlayerBase : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private PlayerData data;
    
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    
    // Internal references
    protected PhotonView photonView;
    private Vector3 rotation = Vector3.zero;
    private float cameraAngle;
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Unity Methods
    // ──────────────────────────────────────────────────────────────────────────────
    protected virtual void Awake() { photonView = GetComponent<PhotonView>(); }

    protected virtual void Start()
    {
        if (!photonView.IsMine)
        {
            DisableCamera();
        }
    }
    protected virtual void Update()
    {
        if (!photonView.IsMine) return;
        HandleMovement();
        HandleLook();
    }
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Movement & Look
    // ──────────────────────────────────────────────────────────────────────────────

    protected virtual void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0f, v);

        Vector3 movement = inputDir * (data.NormalSpeed * Time.deltaTime);
        transform.Translate(movement, Space.Self);
    }

    protected virtual void HandleLook()
    {
        //100f va a ser por ahora la sensibilidad pero esto deberia cambiar para cada jugador
        rotation.x = Input.GetAxis("Mouse X") * Time.deltaTime * 100f;
        rotation.y = Input.GetAxis("Mouse Y") * Time.deltaTime * 100f;

        cameraAngle += rotation.y;
        cameraAngle = Mathf.Clamp(cameraAngle, -70, 70);

        transform.Rotate(Vector3.up * rotation.x);
        playerCamera.transform.localRotation = Quaternion.Euler(-cameraAngle, 0, 0);
    }
    
    private void DisableCamera()
    {
        if (playerCamera == null) return;

        playerCamera.enabled = false;

        if (playerCamera.TryGetComponent(out AudioListener listener))
        {
            listener.enabled = false;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // RPC
    // ──────────────────────────────────────────────────────────────────────────────
    [PunRPC]
    public virtual void TeleportToLocation(Vector3 newPosition) { transform.position = newPosition; }
}
