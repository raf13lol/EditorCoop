using System.IO;
using BepInEx;
using BepInEx.Configuration;
#if !BPE5
using BepInEx.Unity.Mono;
#endif
using EditorCoop.Functionality;
using EditorCoop.Functionality.Network;
using HarmonyLib;
using Network.Packets;
using Network.Steam;
using UnityEngine;

namespace EditorCoop;

[BepInProcess("Rhythm Doctor.exe")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Entry : BaseUnityPlugin
{
    public const bool IsBPE5 =
#if !BPE5 
    false;
#else 
    true;
#endif

#if !BPE5
    public const string DLLName = "com.rhythmdr.editorcoop.dll";
#else
    public const string DLLName = "com.rhythmdr.bpe5editorcoop.dll";
#endif

    public static string UserDataFolder = Path.Combine(Application.dataPath.Replace("Rhythm Doctor_Data", ""), "User");

    public static ConfigEntry<bool> Enabled;

    public static Harmony HarmonyPatcher;
    public static ConfigFile ConfigurationFile;
    public static PluginInfo PluginInfo;

    public void Awake()
    {
        Enabled = Config.Bind("", "Enabled", true,
        "If the ability to use the editor co-operatively should be enabled.");

        if (!Enabled.Value)
        {
            // Logger.LogMessage("EditorCoop");
            return;
        }

        // Functionality init
        PacketBinary.Providers.Add(new RDPacketProvider());
        Connection.PacketTypeEnum = typeof(PacketType);

        Lobby.PacketReadCallback += PacketReadHandler.OnPacketRead;
        Lobby.PacketReadCallback += PacketReadHandler.HandleReplication;

        // Mod patching init
        HarmonyPatcher = new("EC");
        ConfigurationFile = Config;
        PluginInfo = Info;
        Patch.Log = Logger;

        TestPatch.Patch(HarmonyPatcher);
    }
}

