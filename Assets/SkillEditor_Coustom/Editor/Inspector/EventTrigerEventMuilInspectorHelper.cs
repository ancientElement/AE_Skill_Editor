using UnityEditor;
using ARPG_AE_JOKER.SkillEditor;
using ARPG_AE_JOKER;
using System.Collections.Generic;

public class EventTrigerEventMuilInspectorHelper : ScriptableObjectSingleton<EventTrigerEventMuilInspectorHelper>
{
    public EventTrigerEventMuil skillFrameEventBase;

    public EventNameListSO eventNameListSo;

    public List<string> eventNames
    {
        get
        {
            if (eventNameListSo == null)
            { return new List<string>(); }
            return eventNameListSo.eventNameList;
        }
    }

    public void Inspector(SkillFrameEventBase skillFrameEventBase)
    {
        this.skillFrameEventBase = (EventTrigerEventMuil)skillFrameEventBase;
        Selection.activeObject = this;
    }

    public void OnValidate()
    {
        SkillEditorWindow.Instance?.ResetView();
    }
}
