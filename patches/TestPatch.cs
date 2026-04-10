using HarmonyLib;
using Network.Steam;
using UnityEngine;

namespace EditorCoop;

public class TestPatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(TestSteamLobbyPatch));
        patcher.PatchAll(typeof(TestCreateSteamLobbyPatch));
    }

    [HarmonyPatch(typeof(SteamIntegration), nameof(SteamIntegration.Setup))]
    public class TestSteamLobbyPatch
    {
        public static void Postfix()
        {
            Lobby.Initialise();
        }
    }

    [HarmonyPatch(typeof(RDBase), "Update")]
    public class TestCreateSteamLobbyPatch
    {
        public static void Postfix()
        {
            if (!Input.GetKey(KeyCode.F6))
                return;
            Lobby.CreateLobby();
        }
    }
}