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
        Patch.Log.LogMessage($"Packet {packet.Type} received");

        switch (packet.Type)
        {
            case PacketType.Test:
                Patch.Log.LogMessage($"TestPacket: Rand string = {((TestPacket)packet).RandomString}");
                break;
            case PacketType.Ping:
                Patch.Log.LogMessage($"PingPacket received");
                break;
            default:
                Patch.Log.LogWarning($"No handler for {packet.Type}");
                break;
        }
    }
    
    public static void HandleReplication(Packet packet, SteamNetworkingIdentity originalUser)
    {
        Patch.Log.LogMessage("HR called");        
        if (!Lobby.IsHost)
            return;

        // some packets are not to be replicated
        switch (packet.Type)
        {
            case PacketType.Ping:
            case PacketType.__Template:
                return;
        }

        foreach (SteamNetworkingIdentity user in Lobby.Connection.Users)
        {
            if (originalUser.Equals(user))
                continue;
            Lobby.Connection.SendPacket(packet, user);
        }
    }
}