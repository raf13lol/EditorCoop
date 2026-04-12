using EditorCoop.Functionality.Network.Packets;
using Network.Steam;

namespace EditorCoop.Functionality.Handlers;

public class CheckVersionHandler
{
    public static void Run(CheckVersionPacket packet)
    {
        if (packet.IsEditorLobby 
        && packet.Commit == Releases.buildCommit 
        && packet.PluginVersion == MyPluginInfo.PLUGIN_VERSION)
            return;

        Lobby.LeaveLobby();        
    }
}