using System.Linq;
using EditorCoop.Functionality;
using EditorCoop.Functionality.Handlers.Events;
using EditorCoop.Functionality.Packets.Assets;
using HarmonyLib;
using Network.Steam;
using RDLevelEditor;
using UnityEngine;

namespace EditorCoop.Patches;

public class TestingPatch : Patch
{
    [HarmonyPatch(typeof(scnEditor), "Update")]
    public class TestingUpdatePatch
    {
        public static void Postfix(scnEditor __instance)
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                CreateEventsHandler.Run(new()
                {
                    Events = [
                        new()
                        {
                            UID = 123,
                            EventType = LevelEventType.MoveRow,

                            Bar = 2,
                            Beat = 3,

                            Tab = Tab.Actions,
                            Y = 0,

                            Row = 0,
                            Target = "",

                            CopyEventUID = null
                        }
                    ]
                });
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                MoveEventsHandler.Run(new()
                {
                    Movements = [
                        new()
                        {
                            UID = 123,

                            Bar = 1,
                            Beat = 1,

                            Y = 2,

                            Row = 0,
                            Target = ""
                        }
                    ]
                });
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                DeleteEventsHandler.Run(new()
                {
                    UIDs = [123]
                });
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                EditEventsHandler.Run(new()
                {
                    Edits = [
                        new()
                        {
                            FoundUID = true,
                            LevelEventControl = UIDHolder.UIDToEventControl[123],
                            LevelEvent = UIDHolder.UIDToEventControl[123].levelEvent,
                            PropertiesInfo = UIDHolder.UIDToEventControl[123].levelEvent.info.propertiesInfo,
                            
                            UID = 123,
                            PropertiesChanged =
                            [
                                new()
                                {
                                    Index = 0,
                                    Value = MoveRowTarget.Heart
                                }
                            ]
                        }
                    ]
                });
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                LevelEventControl_Base control = __instance.eventControls.First(c => c.levelEvent.type == LevelEventType.PlaySong);
                string testFileName = ((LevelEvent_PlaySong)control.levelEvent).song.filename;
                foreach (SyncAssetsPacket packet in AssetManager.CreateSyncAssetsPackets([testFileName]))
                    Lobby.SendPacketToAll(packet, MessageFlags.Reliable | MessageFlags.NoNagle);
            }
        }
    }
}