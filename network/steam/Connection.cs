using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EditorCoop;
using Network.Packets;
using EditorCoop.Functionality.Network.Packets;
using Steamworks;

namespace Network.Steam;

public class Connection : IDisposable
{
    public static Type PacketTypeEnum;

    public List<SteamNetworkingIdentity> Users = [];

    public delegate void PacketReadCallback(Packet packet, SteamNetworkingIdentity user);
    public event PacketReadCallback OnPacketRead;

    public List<CancellationTokenSource> PingCancellationTokenSources;

    private readonly IntPtr[] MessagePointers = new IntPtr[16];

    public Connection()
    {
        PingCancellationTokenSources = [];
    }

    public Connection(SteamNetworkingMessagesSessionRequest_t sessionRequest) : this()
    {
        SteamNetworkingIdentity user = sessionRequest.m_identityRemote;
        SteamNetworkingMessages.AcceptSessionWithUser(ref user);

        SteamNetworkingIdentity clone = new();
        clone.SetSteamID64(user.GetSteamID64());

        Users.Add(clone);
        AddPingTaskForUser(user);
    }
    
    public void AddPingTaskForUser(SteamNetworkingIdentity user)
    {
        CancellationTokenSource cancellationTokenSource = new();
        PingCancellationTokenSources.Add(cancellationTokenSource);

        Task.Run(async delegate
        {
            // Don't like this but it's the cleanest shit we got
            bool firstRun = true;
            while (cancellationTokenSource.IsCancellationRequested)
            {
                if (!firstRun)
                    SendPacket(new PingPacket(), user);

                await Task.Delay(60 * 1000, cancellationTokenSource.Token);
                firstRun = false;
            }
        }, cancellationTokenSource.Token);
    }

    public void SendCheckVersionPacket(SteamNetworkingIdentity user)
    {
        Users.Add(user);
        AddPingTaskForUser(user);

        // also requests a session
        SendPacket(new CheckVersionPacket()
        {
            IsEditorLobby = true,
            Commit = Releases.buildCommit,
            PluginVersion = MyPluginInfo.PLUGIN_VERSION
        }, user);
    }

    public void Dispose()
    {
        for (int i = 0; i < Users.Count; i++)
        {
            SteamNetworkingIdentity user = Users[i];
            SteamNetworkingMessages.CloseSessionWithUser(ref user);

            CancellationTokenSource cancellationTokenSource = PingCancellationTokenSources[i];
            cancellationTokenSource.Cancel();
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
            
            CancellationTokenSource cancellationTokenSource = PingCancellationTokenSources[i];
            cancellationTokenSource.Cancel();

            Users.RemoveAt(i);
            PingCancellationTokenSources.RemoveAt(i);
            return;
        }
    }

    public void ReadPackets()
    {
        int messageCount = SteamNetworkingMessages.ReceiveMessagesOnChannel(0, MessagePointers, MessagePointers.Length);
        for (int i = 0; i < messageCount; i++)
        {
            IntPtr pointer = MessagePointers[i];
            SteamNetworkingMessage_t message = SteamNetworkingMessage_t.FromIntPtr(pointer);

            byte[] buffer = new byte[message.m_cbSize];
            Marshal.Copy(message.m_pData, buffer, 0, message.m_cbSize);

            Packet packet = Packet.Decode(buffer, PacketTypeEnum);
            OnPacketRead.Invoke(packet, message.m_identityPeer);

            SteamNetworkingMessage_t.Release(pointer);
        }
    }

    public bool SendPacket(Packet packet, SteamNetworkingIdentity user)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        packet.Encode(writer);

        byte[] data = stream.GetBuffer();
        EResult result;

        unsafe
        {
            fixed (byte* pointer = data)
                result = SteamNetworkingMessages.SendMessageToUser(ref user, (IntPtr)pointer, (uint)data.Length, (int)MessageFlags.Reliable, 0);
        }

        return result == EResult.k_EResultOK;
    }
}