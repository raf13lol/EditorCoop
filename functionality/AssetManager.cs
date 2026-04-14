using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using EditorCoop.Functionality.Packets.Assets;
using EditorCoop.Patches;
using RDLevelEditor;

namespace EditorCoop.Functionality;

public class AssetManager
{
    public const int MaxMessageSize = 512 * (1024 - 1); // - 1 is to be better safe than sorry

    public static List<SyncAssetsPacket> CreateSyncAssetsPackets(List<string> fileNames)
    {
        string levelFolder = Path.GetDirectoryName(scnEditor.instance.openedFilePath);

        using MemoryStream stream = new();
        ZipArchive zip = new(stream, ZipArchiveMode.Create, true);

        foreach (string fileName in fileNames)
        {
            string filePath = Path.Combine(levelFolder, fileName);
            if (File.Exists(filePath))
                zip.CreateEntryFromFile(filePath, fileName);
        }

        zip.Dispose();

        byte[] zipFile = stream.ToArray();
        int fragments = (int)Math.Floor((double)zipFile.Length / MaxMessageSize) + 1;

        List<SyncAssetsPacket> packets = [];
        int uid = RXRandom.Int();

        int offset = 0;
        for (int i = 0; i < fragments; i++)
        {
            int length = Math.Min(zipFile.Length - offset, MaxMessageSize);
            packets.Add(new()
            {
                UID = uid,
                Finished = i == (fragments - 1),
                Fragment = zipFile[offset..(offset + length)]
            });

            offset += MaxMessageSize;
        }

        return packets;
    }
}