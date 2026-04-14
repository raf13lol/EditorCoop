using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using EditorCoop.Functionality.Packets.Assets;

namespace EditorCoop.Functionality.Handlers;

public class SyncAssetsHandler : Handler
{
    public static Dictionary<int, byte[]> Fragments = [];

    public static void Run(SyncAssetsPacket packet)
    {
        if (!Fragments.TryGetValue(packet.UID, out byte[] bytes))
        {
            bytes = [];
            Fragments[packet.UID] = bytes;
        }
        
        bytes = [.. bytes, .. packet.Fragment];
        if (!packet.Finished)
            return;

        using MemoryStream zipStream = new(bytes);
        using ZipArchive zip = new(zipStream);

        zip.ExtractToDirectory(Path.GetDirectoryName(Editor.openedFilePath));
        Fragments.Remove(packet.UID);
    }
}