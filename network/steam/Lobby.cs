using System;
using System.Collections.Generic;
using EditorCoop;
using Network.Packets;
using Steamworks;

namespace Network.Steam;

public class Lobby
{
    public static bool InLobby = false;
    public static bool IsHost = false;

    public static int LobbyCount => Connections.Count;
    public static List<Connection> Connections = [];

    public static CSteamID SelfSteamID = new(0ul);
    public static CSteamID HostSteamID = new(0ul);
    public static CSteamID LobbySteamID = new(0ul);

    public static Connection.PacketReadCallback PacketReadCallback
    {
        get => _packetReadCallback;
        set
        {
            foreach (Connection c in Connections)
            {
                if (_packetReadCallback != null)
                    c.OnPacketRead -= _packetReadCallback;
                c.OnPacketRead += value;
            }

            _packetReadCallback = value;
        }
    }

    private static Connection.PacketReadCallback _packetReadCallback;

    private static CallResult<LobbyCreated_t> LobbyCreatedResult;
    
    private static Callback<LobbyEnter_t> LobbyEnterCallback;
    private static Callback<LobbyChatUpdate_t> LobbyChatUpdateCallback;
    private static Callback<GameLobbyJoinRequested_t> LobbyJoinRequestedCallback;

    private static Callback<SteamNetworkingMessagesSessionRequest_t> SessionRequestCallback;

    public static void Initialise()
    {
        if (!SteamIntegration.initialized)
            return;

        SelfSteamID = SteamUser.GetSteamID();

        LobbyCreatedResult = CallResult<LobbyCreated_t>.Create(LobbyCreated);
    
        LobbyEnterCallback = Callback<LobbyEnter_t>.Create(LobbyEnter);
        LobbyChatUpdateCallback = Callback<LobbyChatUpdate_t>.Create(LobbyChatUpdate);
        LobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(LobbyJoinRequested);
        
        SessionRequestCallback = Callback<SteamNetworkingMessagesSessionRequest_t>.Create(SessionRequest);

        CheckForCommandLineJoin();
    }

    private static void CheckForCommandLineJoin()
	{
		string[] args = Environment.GetCommandLineArgs();
		int index = args.IndexOf("+connect_lobby");
		if (index > -1 && ++index < args.Length)
		{
			JoinLobby(ulong.Parse(args[index]));
			return;
		}
	}

    public static void CreateLobby()
    {
        if (!SteamIntegration.initialized)
            return;
        if (InLobby)
            return;

        SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 16);
		LobbyCreatedResult.Set(handle);
    }

    public static void JoinLobby(ulong lobbyID)
	{
		if (!SteamIntegration.initialized)
			return;
        if (InLobby)
            return;

		SteamMatchmaking.JoinLobby(new(lobbyID));
	}

    public static void LeaveLobby()
	{
        if (!InLobby)
            return;

		SteamMatchmaking.LeaveLobby(LobbySteamID);
        InLobby = false;

        foreach (Connection connection in Connections)
            connection.Dispose();
        Connections.Clear();
    }

    private static void LobbyCreated(LobbyCreated_t result, bool error)
    {
        Patch.Log.LogMessage($"Lobby {result.m_ulSteamIDLobby} created");
        LobbySteamID = new(result.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyJoinable(LobbySteamID, true);
    }

    private static void LobbyEnter(LobbyEnter_t message)
    {
        Patch.Log.LogMessage($"Lobby {message.m_ulSteamIDLobby} entered");
        InLobby = true;
        LobbySteamID = new(message.m_ulSteamIDLobby);

        HostSteamID = SteamMatchmaking.GetLobbyOwner(LobbySteamID);
        IsHost = SelfSteamID == HostSteamID;
    }

    private static void LobbyChatUpdate(LobbyChatUpdate_t message)
    {
        Patch.Log.LogMessage($"Lobby {message.m_ulSteamIDLobby} {message.m_rgfChatMemberStateChange} {message.m_ulSteamIDUserChanged} update");
        EChatMemberStateChange stateChange = (EChatMemberStateChange)message.m_rgfChatMemberStateChange;
        if ((stateChange & EChatMemberStateChange.k_EChatMemberStateChangeLeft) > 0)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                Connection connection = Connections[i];
                if (connection.User.GetSteamID64() != message.m_ulSteamIDUserChanged)
                    continue;
                Connections.RemoveAt(i--);
                connection.Dispose();
            }
            return;
        }
        
        if ((stateChange & EChatMemberStateChange.k_EChatMemberStateChangeEntered) > 0)
        {
            if (!IsHost)
                return;
            Connections.Add(SetConnectionOnPacketReadCallback(new(message.m_ulSteamIDUserChanged)));
            return;
        }
    }

    private static void LobbyJoinRequested(GameLobbyJoinRequested_t message)
    {
        Patch.Log.LogMessage($"Lobby {message.m_steamIDLobby.m_SteamID} {message.m_steamIDFriend.m_SteamID} requested to join");
        if (InLobby)
			LeaveLobby();
		JoinLobby(message.m_steamIDLobby.m_SteamID);
    }

    private static void SessionRequest(SteamNetworkingMessagesSessionRequest_t message)
    {
        Patch.Log.LogMessage($"Lobby {message.m_identityRemote.GetSteamID64()} session request");
        Connections.Add(SetConnectionOnPacketReadCallback(new(message)));
    }

    private static Connection SetConnectionOnPacketReadCallback(Connection connection)
    {
        connection.OnPacketRead += delegate(Packet packet, ulong from)
        {
            foreach (Connection c in Connections)
            {
                if (c.UserID != from)
                    c.SendPacket(packet);
            }    
        };
        connection.OnPacketRead += PacketReadCallback;

        return connection;
    }
}