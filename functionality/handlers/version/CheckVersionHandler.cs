using EditorCoop.Functionality.Packets.Version;
using Network.Steam;

namespace EditorCoop.Functionality.Handlers.Version;

public class CheckVersionHandler : Handler
{
    public static void Run(CheckVersionPacket packet)
    {
        if (packet.IsEditorLobby && packet.Commit == Releases.buildCommit)
            return;
            
        Lobby.LeaveLobby();        
    }
}