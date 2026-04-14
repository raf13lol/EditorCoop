using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
#if !BPE5
using BepInEx.Unity.Mono;
#endif
using EditorCoop.Functionality;
using EditorCoop.Patches;
using EditorCoop.Patches.Events;
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
            return;

        // Functionality init
        PacketBinary.Providers.Add(new RDPacketProvider());

        Assembly thisAssembly = GetType().Assembly;
        Type[] assemblyTypes = thisAssembly.GetTypes();

        foreach (Type type in assemblyTypes)
        {
            if (type.GetCustomAttribute<PacketAttribute>() == null || type.Name.Contains("Template"))
                continue;

            string packetTypeName = type.Name.Replace("Packet", "");
            Type handlerType = null;

            try
            {
                handlerType = assemblyTypes.First(t => t.Name == $"{packetTypeName}Handler");
            }
            catch
            { 
                // pointless catch but whatever c#
            }

            if (handlerType == null)
            {
                Logger.LogWarning($"{type.Name} doesn't have a handler. This packet type will not be registered.");
                continue;
            }
            MethodInfo handlerMethod = handlerType.GetMethod("Run", AccessTools.all);

            Encoding.Register(type, packet => handlerMethod.Invoke(null, [packet]));
        }

        Lobby.DataReadCallback += ReplicationHandler.Run;

        // Mod patching init
        HarmonyPatcher = new("EC");
        ConfigurationFile = Config;
        PluginInfo = Info;
        Patches.Patch.Log = Logger;

        SteamPatch.Patch(HarmonyPatcher);
        TestingPatch.Patch(HarmonyPatcher);

        EventCreationPatch.Patch(HarmonyPatcher);
        EventDeletionPatch.Patch(HarmonyPatcher);
        EventEditPatch.Patch(HarmonyPatcher);
        EventMovementPatch.Patch(HarmonyPatcher);
    }
}

