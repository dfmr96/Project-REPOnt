namespace Interfaces
{
    using Photon.Pun;

    public interface IInteractable
    {
        void Interact(PhotonView actorView, int optionalId = -1);
    }
}