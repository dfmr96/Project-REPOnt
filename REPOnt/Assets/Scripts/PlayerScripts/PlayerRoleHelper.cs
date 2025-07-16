using Photon.Pun;
using Photon.Realtime;
using System;

public static class PlayerRoleHelper
{
    private const string RoleKey = "Role";
    private const string GhostRole = "Ghost";
    public static Player GetGhostPlayer()
    {
        return Array.Find(PhotonNetwork.PlayerList, p => p.CustomProperties.TryGetValue(RoleKey, out object role) && role != null && role.ToString() == GhostRole);
    }

    public static bool IsLocalPlayerGhost()
    {
        var ghost = GetGhostPlayer();
        return ghost != null && ghost == PhotonNetwork.LocalPlayer;
    }
}
