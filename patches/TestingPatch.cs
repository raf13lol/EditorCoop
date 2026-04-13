using EditorCoop.Functionality.Handlers.Events;
using HarmonyLib;
using RDLevelEditor;
using UnityEngine;

namespace EditorCoop.Patches;

public class TestingPatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(TestingUpdatePatch));
    }

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
                            EventUID = 123,
                            EventType = LevelEventType.Move,

                            Bar = 2,
                            Beat = 3,

                            Tab = Tab.Sprites,
                            Y = 0,
                            SubRowY = 0,

                            Row = null,
                            Target = __instance.spritesData[0].spriteId,

                            CopyEventUID = null
                        }
                    ]
                });
            }
        }
    }
}