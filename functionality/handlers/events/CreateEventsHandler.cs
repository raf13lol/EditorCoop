using System;
using EditorCoop.Compatibility;
using EditorCoop.Functionality.Data.Events;
using EditorCoop.Functionality.Packets.Events;
using RDLevelEditor;
using UnityEngine;

namespace EditorCoop.Functionality.Handlers.Events;

public class CreateEventsHandler : Handler
{
    public static void Run(CreateEventsPacket packet)
    {
        foreach (EventCreationData newEventData in packet.Events)
        {
            Type newEventType = Type.GetType($"RDLevelEditor.LevelEvent_{newEventData.EventType}, Assembly-CSharp");
            LevelEvent_Base newEvent = (LevelEvent_Base)Activator.CreateInstance(newEventType);

            newEvent.uid = newEventData.EventUID;

            newEvent.bar = newEventData.Bar;
            newEvent.beat = newEventData.Beat;

            newEvent.tab = newEventData.Tab;
            newEvent.y = newEventData.Y;
            if (PluginCompatibility.RDEditorPlusDetected)
                RDEditorPlusCompatibility.SetEventSubRowY(newEvent, newEventData.SubRowY);

            if (newEventData.Row is int row)
                newEvent.row = row;
            newEvent.target = newEventData.Target;

            if (newEventData.CopyEventUID is int copyEventUID
            && UIDHolder.UIDToEvent.TryGetValue(copyEventUID, out LevelEvent_Base copyEvent))
            {
                newEvent.CopyFromInternal(copyEvent);
                if (newEvent is LevelEvent_AddOneshotBeat oneshot)
                    oneshot.CapXPosForPositiveFreezeBurnCueTime();
            }

            if (newEventData.Tab == Tab.Sprites)
                newEvent.row = SpriteHeader.GetSpriteDataIndex(newEventData.Target);

            newEvent.OnCreate();

            LevelEventControl_Base control = Editor.CreateEventControl(newEvent, newEventData.Tab, true);
            control.UpdateUI();
            Editor.UpdateTimelineAccordingToLevelEventType(newEventData.EventType);

            FlagUnsavedChanges();
        }
    }
}