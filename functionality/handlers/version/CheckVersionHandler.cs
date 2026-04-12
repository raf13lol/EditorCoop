using EditorCoop.Functionality.Network.Packets;
using Network.Steam;

namespace EditorCoop.Functionality.Handlers.Version;

public class CheckVersionHandler : Handler
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