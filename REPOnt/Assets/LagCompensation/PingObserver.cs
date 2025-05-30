using Photon.Pun;

public static class PingObserver
{
    public static int CurrentPing => PhotonNetwork.GetPing();
}
