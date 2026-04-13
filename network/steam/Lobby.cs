using System;
using EditorCoop.Patches;
using Network.Packets;
using Steamworks;

namespace Network.Steam;

public class Lobby
{
    public static bool InLobby = false;
    public static bool IsHost = false;

    public static Connection Connection;

    public static CSteamID SelfSteamID = new(0ul);
    public static CSteamID HostSteamID = new(0ul);
    public static CSteamID LobbySteamID = new(0ul);

    public static event Connection.PacketReadCallback PacketReadCallback;
    public static event Connection.DataReadCallback DataReadCallback;

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

        Connection.Dispose();
        Connection = null;
    }

    public static bool SendPacketToAll(Packet packet)
    {
        if (Connection == null)
            return false;

        byte[] data = Packet.Encode(packet);
        bool result = true;
        foreach (SteamNetworkingIdentity user in Connection.Users)
           result = Connection.Send(data, user) && result;
        return result;
    }

    private static void LobbyCreated(LobbyCreated_t result, bool error)
    {
        Patch.Log.LogMessage($"Lobby {result.m_ulSteamIDLobby} created");
        LobbySteamID = new(result.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyJoinable(LobbySteamID, true);

        Connection = SetConnectionOnPacketReadCallback(new());
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
        EChatMemberStateChange stateChange = (EChatMemberStateChange)message.m_rgfChatMemberStateChange;
        Patch.Log.LogMessage($"Lobby {message.m_ulSteamIDLobby} {stateChange} {message.m_ulSteamIDUserChanged} update");
        CSteamID userAffected = (CSteamID)message.m_ulSteamIDUserChanged;
        if ((stateChange & EChatMemberStateChange.k_EChatMemberStateChangeLeft) > 0)
        {
            if (!IsHost && userAffected == HostSteamID)
                LeaveLobby();
            else if (IsHost)
                Connection.DisposeUserID(message.m_ulSteamIDUserChanged);
            return;
        }

        if ((stateChange & EChatMemberStateChange.k_EChatMemberStateChangeEntered) > 0)
        {
            if (!IsHost)
                return;
            SteamNetworkingIdentity newUser = new();
            newUser.SetSteamID64(message.m_ulSteamIDUserChanged); 
            Connection.SendSessionInitPacket(newUser);
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
        Connection = SetConnectionOnPacketReadCallback(new(message));
    }

    private static Connection SetConnectionOnPacketReadCallback(Connection connection)
    {
        connection.OnPacketRead += PacketReadCallback.Invoke;
        connection.OnDataRead += DataReadCallback.Invoke;
        return connection;
    }
}