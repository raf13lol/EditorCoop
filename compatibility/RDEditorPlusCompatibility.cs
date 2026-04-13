using RDEditorPlus.ExtraData;
using RDLevelEditor;

namespace EditorCoop.Compatibility;

public class RDEditorPlusCompatibility
{
    public static void SetEventSubRowY(LevelEvent_Base levelEvent, int subRowY)
        => SubRowStorage.Instance.SetSubRow(levelEvent, subRowY);
}