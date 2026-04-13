using EditorCoop.Functionality;
using EditorCoop.Functionality.Packets.Events;
using HarmonyLib;
using Network.Steam;
using RDLevelEditor;

namespace EditorCoop.Patches.Events;

public class EventCreationPatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(AddNewEventControlPatch));
    }

    [HarmonyPatch(typeof(scnEditor), nameof(scnEditor.AddNewEventControl))]
    public class AddNewEventControlPatch
    {
        public static void Postfix(scnEditor __instance, LevelEventControl_Base eventControl)
        {
            if (HandlerInProgress || eventControl.levelEvent.isBaseEvent)
                return;

            LevelEvent_Base levelEvent = eventControl.levelEvent;
            Lobby.SendPacketToAll(new CreateEventsPacket()
            {
                Events = [
                    new()
                    {
                        UID = levelEvent.uid,
                        EventType = levelEvent.type,

                        Bar = levelEvent.bar,
                        Beat = levelEvent.beat,

                        Tab = levelEvent.tab,
                        Y = levelEvent.y,
                        SubRowY = 0,

                        Row = levelEvent.row,
                        Target = levelEvent.target,

                        CopyEventUID = null
                    }
                ]
            });

            UIDHolder.StoreEventControl(eventControl);
        }
    }
}