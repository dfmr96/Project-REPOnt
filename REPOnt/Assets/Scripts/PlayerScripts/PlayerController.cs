using Photon.Pun;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerData data;
        [SerializeField] private Camera playerCamera;
        private PhotonView photonView;
        private Vector3 rotation = Vector3.zero;
        private float cameraAngle;
        private void Awake() { photonView = GetComponent<PhotonView>(); }

        void Update()
        {
            if (!photonView.IsMine) 
            {
                playerCamera.enabled = false;
                playerCamera.GetComponent<AudioListener>().enabled = false;
                return;
            }
            Move();
            Look();
        }

        private void Move() 
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 move = new Vector3(h, 0, v);
            transform.Translate(move * (Time.deltaTime * data.NormalSpeed));
        }

        private void Look() 
        {
            //5f va a ser por ahora la sensibilidad pero esto deberia cambiar para cada jugador
            rotation.x = Input.GetAxis("Mouse X") * Time.deltaTime * 100f;
            rotation.y = Input.GetAxis("Mouse Y") * Time.deltaTime * 100f;

            cameraAngle += rotation.y;
            cameraAngle = Mathf.Clamp(cameraAngle, -70, 70);

            transform.Rotate(Vector3.up * rotation.x);
            playerCamera.transform.localRotation = Quaternion.Euler(-cameraAngle, 0, 0);
        }
    
        [PunRPC]
        public void TeleportToLocation(Vector3 newPosition)
        {
            transform.position = newPosition;
        }
    }
}
