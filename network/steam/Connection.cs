using System;
using System.IO;
using System.Runtime.InteropServices;
using EditorCoop;
using Network.Packets;
using Network.Packets.Types;
using Steamworks;

namespace Network.Steam;

public class Connection : IDisposable
{
    public SteamNetworkingIdentity User;

    public delegate void ReadPacketCallback(Packet packet);
    public event ReadPacketCallback OnReadPacket;

    private readonly IntPtr[] MessagePointers = new IntPtr[16];

    public Connection(SteamNetworkingIdentity user)
    {
        User = user;
        
        // also requests a session
        SendPacket(new CheckVersionPacket()
        {
            IsEditorLobby = true,
            Commit = Releases.buildCommit,
            PluginVersion = MyPluginInfo.PLUGIN_VERSION
        });
    }

    public Connection(SteamNetworkingMessagesSessionRequest_t sessionRequest)
    {
        User = sessionRequest.m_identityRemote;
        SteamNetworkingMessages.AcceptSessionWithUser(ref User);
    }

    public void Dispose()
    {
        SteamNetworkingMessages.CloseSessionWithUser(ref User);
        System.GC.SuppressFinalize(this);
    }

    public void UpdateCallbacks()
    {
        if (OnReadPacket == null)
            return;
        foreach (Packet packet in ReadPackets())
            OnReadPacket.Invoke(packet);
    }

    public Packet[] ReadPackets()
    {
        int messageCount = SteamNetworkingMessages.ReceiveMessagesOnChannel(0, MessagePointers, MessagePointers.Length);
        if (messageCount <= 0)
            return [];

        Packet[] packets = new Packet[messageCount];
        for (int i = 0; i < messageCount; i++)
        {
            IntPtr pointer = MessagePointers[i];
            SteamNetworkingMessage_t message = SteamNetworkingMessage_t.FromIntPtr(pointer);

            byte[] buffer = new byte[message.m_cbSize];
			Marshal.Copy(message.m_pData, buffer, 0, message.m_cbSize);

            packets[i] = Packet.Decode(buffer);

			SteamNetworkingMessage_t.Release(pointer);
        }

        return packets;
    }

    public bool SendPacket(Packet packet)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        packet.Encode(writer);

        byte[] data = stream.GetBuffer();
        EResult result;

        unsafe
        {
            fixed (byte* pointer = data) 
                result = SteamNetworkingMessages.SendMessageToUser(ref User, (IntPtr)pointer, (uint)data.Length, (int)MessageFlags.Reliable, 0);
        }

        return result == EResult.k_EResultOK;
    }
}