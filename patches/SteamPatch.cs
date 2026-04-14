using EditorCoop.Functionality.Packets.Version;
using HarmonyLib;
using Network.Steam;
using UnityEngine;

namespace EditorCoop.Patches;

public class SteamPatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(InitSteamPatch));
        patcher.PatchAll(typeof(SetSessionInitPacketPatch));
        patcher.PatchAll(typeof(SteamUpdateCallbacksPatch));
    }

    [HarmonyPatch(typeof(SteamIntegration), nameof(SteamIntegration.Setup))]
    public class InitSteamPatch
    {
        public static void Postfix()
            => Lobby.Initialise();
    }

    [HarmonyPatch(typeof(RDStartup), nameof(RDStartup.Setup))]
    public class SetSessionInitPacketPatch
    {
        public static void Postfix()
        {
            Connection.SessionInitPacket = new CheckVersionPacket()
            {
                IsEditorLobby = true,
                Commit = Releases.buildCommit
            };
        }
    }

    [HarmonyPatch(typeof(scnBase), "Update")]
    public class SteamUpdateCallbacksPatch
    {
        public static void Postfix()
        {
            SteamIntegration.CheckCallbacks();
            Lobby.Connection?.ReadPackets();

            if (Input.GetKey(KeyCode.F6))
                Lobby.CreateLobby();
        }
    }
}