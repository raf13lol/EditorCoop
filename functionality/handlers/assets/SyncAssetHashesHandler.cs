using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EditorCoop.Functionality.Data.Assets;
using EditorCoop.Functionality.Packets.Assets;

namespace EditorCoop.Functionality.Handlers;

public class SyncAssetHashesHandler : Handler
{
    public static void Run(SyncAssetHashesPacket packet)
    {
        List<string> assetsToRequest = [];

        foreach (AssetHashData hashData in packet.Hashes)
        {
            string levelFolder = Path.GetDirectoryName(Editor.openedFilePath);
            string filePath = Path.Combine(levelFolder, hashData.Filename);
            if (hashData.Hash == null)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                continue;
            }
            
            if (!File.Exists(filePath))
            {
                assetsToRequest.Add(hashData.Filename);
                return;
            }

            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(File.ReadAllBytes(filePath));
            
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != hashData.Hash[i])
                {
                    assetsToRequest.Add(hashData.Filename);
                    break;
                }
            }
        }

        
    }
}