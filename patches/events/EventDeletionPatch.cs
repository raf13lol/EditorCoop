using EditorCoop.Functionality.Packets.Events;
using HarmonyLib;
using Network.Steam;
using RDLevelEditor;

namespace EditorCoop.Patches;

public class EventDeletionPatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(DeleteEventControlPatch));
    }

    [HarmonyPatch(typeof(scnEditor), nameof(scnEditor.DeleteEventControl))]
    public class DeleteEventControlPatch
    {
        public static void Prefix(LevelEventControl_Base eventControl)
        {
            if (CalledFromHandler || eventControl.levelEvent.isBaseEvent)
                return;

            Lobby.SendPacketToAll(new DeleteEventsPacket()
            {
                UIDs = [eventControl.levelEvent.uid]
            });
        }
    }
}