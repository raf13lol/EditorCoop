using System;
using System.Collections.Generic;
using System.Reflection;
using EditorCoop.Patches;
using Network.Packets;
using Network.Steam;
using Steamworks;

namespace EditorCoop.Functionality;

public class PacketReadHandler
{
    public static Dictionary<PacketType, MethodInfo> HandlePacketMethods = [];

    public static void OnPacketRead(Packet packet, SteamNetworkingIdentity user)
    {
        Patch.Log.LogMessage("OPR called");
        Patch.Log.LogMessage($"Packet {packet.GetType().Name} received");

        PacketType packetType = (PacketType)packet.PacketTypeByte;
        if (!HandlePacketMethods.TryGetValue(packetType, out MethodInfo handlePacketMethod))
        {
            string packetTypeName = packetType.ToString();
            Type? packetHandler = null;
            foreach (Type t in Packet.AssemblyTypes)
            {
                if (t.Name != $"{packetTypeName}Handler")
                    continue;
                packetHandler = t;
                break;
            }

            handlePacketMethod = packetHandler?.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
            HandlePacketMethods[packetType] = handlePacketMethod;
        }

        if (handlePacketMethod == null)
        {
            Patch.Log.LogWarning($"No handler for {packet.GetType().Name}");
            return;
        }

        Patch.HandlerInProgress = true;
        handlePacketMethod.Invoke(null, [packet]);
        Patch.HandlerInProgress = false;
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