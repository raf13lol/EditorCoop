using System.Reflection;
using HarmonyLib;
using RDLevelEditor;

namespace EditorCoop.Functionality.Handlers;

public class Handler
{
    protected static scnEditor Editor => scnEditor.instance;
    private static readonly FieldInfo IsThereChangesToSave = AccessTools.Field(typeof(scnEditor), "theresChangesToSave");

    protected static void FlagUnsavedChanges()
    {
        IsThereChangesToSave.SetValue(Editor, true);
        Editor.unsavedIndicatorCheck = true;
    }

    protected static void InspectorPanelUpdateUI()
    {
        if (Editor.selectedControls.Count == 0)
            Editor.currentTabSection.ShowNothingSelectedPanel();
        else if (Editor.selectedControls.Count > 1)
            Editor.currentTabSection.ShowMultipleSelectedPanel();
        else
            Editor.selectedControl.ShowDataOnInspector();
    }
}