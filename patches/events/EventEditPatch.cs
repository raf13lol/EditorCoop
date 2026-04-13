using EditorCoop.Functionality.Packets.Events;
using HarmonyLib;
using Network.Steam;
using RDLevelEditor;

namespace EditorCoop.Patches;

public class EventEditPatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(SavePatch));
    }

    [HarmonyPatch(typeof(Property), nameof(Property.Save))]
    public class SavePatch
    {
        public static void Prefix(Property __instance, LevelEvent_Base levelEvent, out object __state)
        {
            if (HandlerInProgress)
            {
                __state = 0;
                return;
            }

            __state = __instance.propertyInfo.propertyInfo.GetValue(levelEvent);
        }

        public static void Postfix(Property __instance, LevelEvent_Base levelEvent, object __state)
        {
            if (HandlerInProgress)
                return;

            object change = __instance.propertyInfo.propertyInfo.GetValue(levelEvent);
            if (__state.Equals(change))
                return;
                
            Lobby.SendPacketToAll(new EditEventsPacket()
            {
                Edits = [
                    new()
                    {
                        UID = levelEvent.uid,
                        PropertiesChanged =
                        [
                            new()
                            {
                                Index = levelEvent.info.propertiesInfo.IndexOf(__instance.propertyInfo),
                                Value = change
                            }
                        ],
                        BaseProperties = new()
                        {
                            RunTagNormally = levelEvent.tagRunNormally
                        }
                    }
                ]
            });
        }
    }
}