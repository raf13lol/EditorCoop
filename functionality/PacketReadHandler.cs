using Network.Packets;
using EditorCoop.Functionality.Network.Packets;
using Network.Steam;
using Steamworks;
using EditorCoop.Functionality.Network;

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

        Patch.Log.LogWarning($"No handler for {packet.GetType().Name}");
    }
    
    public static void HandleReplication(byte[] data, SteamNetworkingIdentity originalUser)
    {
        Patch.Log.LogMessage("HR called");        
        if (!Lobby.IsHost)
            return;

        PacketType type = (PacketType)data[0];
        // some packets are not to be replicated
        // if (type == PacketType.Ping)
            // return;

        foreach (SteamNetworkingIdentity user in Lobby.Connection.Users)
        {
            if (originalUser.GetSteamID64() == user.GetSteamID64())
                continue;
            Lobby.Connection.Send(data, user);
        }
    }
}