using HarmonyLib;
using RDLevelEditor;

namespace EditorCoop.Patches;

public class TemplatePatch : Patch
{
    public static void Patch(Harmony patcher)
    {
        patcher.PatchAll(typeof(TemplateTemplatePatch));
    }

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