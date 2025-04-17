using Photon.Pun;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerSync : MonoBehaviourPun, IPunObservable
    {
        private Vector3 networkPosition;
        private Quaternion networkRotation;

        [SerializeField] private float smoothTime = 10f;

        private void Update()
        {
            if (!photonView.IsMine)
            {
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * smoothTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * smoothTime);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
