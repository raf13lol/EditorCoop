using EditorCoop.Functionality.Packets.Events;
using HarmonyLib;
using Network.Steam;
using RDLevelEditor;

namespace EditorCoop.Patches;

public class EventMovementPatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(OnEndDragPatch));
    }

    [HarmonyPatch(typeof(LevelEventControlEventTrigger), nameof(LevelEventControlEventTrigger.OnEndDrag))]
    public class OnEndDragPatch
    {
        public static void Postfix(LevelEventControlEventTrigger __instance)
        {
            if (CalledFromHandler)
                return; 

            LevelEvent_Base levelEvent = __instance.control.levelEvent;
            if (levelEvent.isBaseEvent)
                return;

            Lobby.SendPacketToAll(new MoveEventsPacket()
            {
                Movements = [
                    new()
                    {
                        UID = levelEvent.uid,

                        Bar = levelEvent.bar,
                        Beat = levelEvent.beat,

                        Y = levelEvent.y,

                        Row = levelEvent.row,
                        Target = levelEvent.target
                    }
                ]
            });
        }
    }
}