using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class LagCompensation : MonoBehaviourPun, IPunObservable
{
    public Vector3 networkPosition;
    
    private  Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += rb.velocity * lag;
        }
    }
}
