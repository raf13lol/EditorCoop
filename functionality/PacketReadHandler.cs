using Network.Packets;
using EditorCoop.Functionality.Network.Packets;
using Network.Steam;
using Steamworks;

namespace EditorCoop.Functionality;

public class PacketReadHandler
{
    public static void OnPacketRead(Packet packet, SteamNetworkingIdentity user)
    {
        Patch.Log.LogMessage("OPR called");
        bool isHost = Lobby.IsHost;
        Patch.Log.LogMessage($"Packet {packet.GetType().Name} received");

        if (packet is TestPacket testPacket)
        {
            Patch.Log.LogMessage($"TestPacket: Rand string = {testPacket.RandomString}");
            return;
        }

        if (packet is PingPacket)
        {
            Patch.Log.LogMessage($"PingPacket received");
            return;
        }

        Patch.Log.LogWarning($"No handler for {packet.GetType().Name}");
    }
    
    public static void HandleReplication(Packet packet, SteamNetworkingIdentity originalUser)
    {
        Patch.Log.LogMessage("HR called");        
        if (!Lobby.IsHost)
            return;

        // some packets are not to be replicated
        if (packet is PingPacket)
            return;

        foreach (SteamNetworkingIdentity user in Lobby.Connection.Users)
        {
            if (originalUser.Equals(user))
                continue;
            Lobby.Connection.SendPacket(packet, user);
        }
    }
}