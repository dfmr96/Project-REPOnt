using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NonPhysicsCompensation : MonoBehaviourPun, IPunObservable 
{
     public Vector3 networkPosition = Vector3.zero;
     public Vector3 positionAtLastPacket = Vector3.zero;
     public double currentTime = 0.0;
     public double currentPacketTime = 0.0;
     public double lastPacketTime = 0.0;
     public double timeToReachGoal = 0.0;
	
     void Update ()
     {
          if (!photonView.IsMine)
            UpdateWithNetworkInfo();
     }

     private void UpdateWithNetworkInfo()
     {
          timeToReachGoal = currentPacketTime - lastPacketTime;
          currentTime += Time.deltaTime;
          transform.position = Vector3.Lerp(positionAtLastPacket, networkPosition, (float)(currentTime / timeToReachGoal));
     }

     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
     {
          if (stream.IsWriting) { stream.SendNext(transform.position); }
          else
          {
               currentTime = 0.0;
               positionAtLastPacket = transform.position;
               networkPosition = (Vector3)stream.ReceiveNext();
               lastPacketTime = currentPacketTime;
               currentPacketTime = info.timestamp;
          }
     }
}
