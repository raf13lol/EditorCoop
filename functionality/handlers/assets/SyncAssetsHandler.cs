using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using EditorCoop.Functionality.Packets.Assets;
using EditorCoop.Patches;

namespace EditorCoop.Functionality.Handlers;

public class SyncAssetsHandler : Handler
{
    public static Dictionary<int, List<byte>> Fragments = [];

    public static void Run(SyncAssetsPacket packet)
    {
        if (!Fragments.TryGetValue(packet.UID, out List<byte> bytes))
        {
            bytes = [];
            Fragments[packet.UID] = bytes;
        }
        
        bytes.AddRange(packet.Fragment);
        if (!packet.Finished)
            return;

        Fragments.Remove(packet.UID);

        byte[] zipBytes = [.. bytes];
        
        using MemoryStream zipStream = new(zipBytes);
        using ZipArchive zip = new(zipStream, ZipArchiveMode.Read);

        zip.ExtractToDirectory(Path.GetDirectoryName(Editor.openedFilePath));
    }
}