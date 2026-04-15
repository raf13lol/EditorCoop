using EditorCoop.Patches;
using Network.Steam;
using Steamworks;

namespace EditorCoop.Functionality.Packets;

public class ReplicationHandler
{
    public static void Run(byte[] data, SteamNetworkingIdentity originalUser)
    {
        Patch.Log.LogMessage($"HR called on packet sent from {originalUser.GetSteamID64()}");
        if (!Lobby.IsHost || data[3] == 0x00) // data[3] is ShouldBeReplicated
            return;

        foreach (SteamNetworkingIdentity user in Lobby.Connection.Users)
        {
            if (originalUser.GetSteamID64() == user.GetSteamID64())
                continue;
            Lobby.Connection.Send(data, user);
        }
    }
}