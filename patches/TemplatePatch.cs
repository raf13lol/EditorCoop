using HarmonyLib;
using RDLevelEditor;

namespace EditorCoop.Patches;

public class TemplatePatch : Patch
{
    [HarmonyPatch(typeof(scnEditor), nameof(scnEditor))]
    public class TemplateTemplatePatch
    {
        public static void Postfix(scnEditor __instance)
        {
            if (CalledFromHandler)
                return;
        }
    }
}