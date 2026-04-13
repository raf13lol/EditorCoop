using System.Collections.Generic;
using RDLevelEditor;

namespace EditorCoop.Functionality;

public class UIDHolder
{
    public static Dictionary<int, LevelEventControl_Base> UIDToEventControl = [];

    public static Dictionary<int, Conditional> UIDToConditional = [];
    public static Dictionary<Conditional, int> ConditionalToUID = [];

    public static Dictionary<int, LevelEvent_MakeRow> UIDToRow = [];
    public static Dictionary<LevelEvent_MakeRow, int> RowToUID = [];

    public static Dictionary<string, LevelEvent_MakeSprite> UIDToSprite = [];
    public static Dictionary<LevelEvent_MakeSprite, string> SpriteToUID = [];

    public static void StoreEventControl(LevelEventControl_Base control)
        => UIDToEventControl[control.levelEvent.uid] = control;
}