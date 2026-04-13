using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Network.Packets;
using Steamworks;

namespace Network.Steam;

public class Connection : IDisposable
{
    public static Type PacketTypeEnum;
    public static Packet SessionInitPacket;

    public List<SteamNetworkingIdentity> Users = [];

    public delegate void PacketReadCallback(Packet packet, SteamNetworkingIdentity user);
    public delegate void DataReadCallback(byte[] data, SteamNetworkingIdentity user);
    
    internal event PacketReadCallback OnPacketRead;
    internal event DataReadCallback OnDataRead;

    private readonly IntPtr[] MessagePointers = new IntPtr[16];

    public Connection()
    {
    }

    public Connection(SteamNetworkingMessagesSessionRequest_t sessionRequest) : this()
    {
        SteamNetworkingIdentity user = sessionRequest.m_identityRemote;
        SteamNetworkingMessages.AcceptSessionWithUser(ref user);

        SteamNetworkingIdentity clone = new();
        clone.SetSteamID64(user.GetSteamID64());

        Users.Add(clone);
    }

    public void SendSessionInitPacket(SteamNetworkingIdentity user)
    {
        Users.Add(user);

        // also requests a session
        Send(SessionInitPacket, user);
    }

    public void Dispose()
    {
        for (int i = 0; i < Users.Count; i++)
        {
            SteamNetworkingIdentity user = Users[i];
            SteamNetworkingMessages.CloseSessionWithUser(ref user);
        }

        System.GC.SuppressFinalize(this);
    }

    public void DisposeUserID(ulong userID)
    {
        for (int i = 0; i < Users.Count; i++)
        {
            SteamNetworkingIdentity user = Users[i];
            if (user.GetSteamID64() != userID)
                continue;

            SteamNetworkingMessages.CloseSessionWithUser(ref user);

            Users.RemoveAt(i);
            return;
        }
    }

    public void ReadPackets()
    {
        int messageCount = SteamNetworkingMessages.ReceiveMessagesOnChannel(1, MessagePointers, MessagePointers.Length);
        for (int i = 0; i < messageCount; i++)
        {
            IntPtr pointer = MessagePointers[i];
            SteamNetworkingMessage_t message = SteamNetworkingMessage_t.FromIntPtr(pointer);

            byte[] buffer = new byte[message.m_cbSize];
            Marshal.Copy(message.m_pData, buffer, 0, message.m_cbSize);
            
            Packet packet = Packet.Decode(buffer, PacketTypeEnum);
            OnPacketRead.Invoke(packet, message.m_identityPeer);
            OnDataRead.Invoke(buffer, message.m_identityPeer);

            SteamNetworkingMessage_t.Release(pointer);
        }
    }

    public bool Send(Packet packet, SteamNetworkingIdentity user)
        => Send(Packet.Encode(packet), user);

    public bool Send(byte[] data, SteamNetworkingIdentity user)
    {
        EResult result;

        unsafe
        {
            fixed (byte* pointer = data)
                result = SteamNetworkingMessages.SendMessageToUser(ref user, (IntPtr)pointer, (uint)data.Length, (int)MessageFlags.Reliable, 1);
        }

        return result == EResult.k_EResultOK;
    }
}