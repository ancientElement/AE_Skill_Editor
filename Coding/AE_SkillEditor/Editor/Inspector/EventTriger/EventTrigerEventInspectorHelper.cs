using System.Collections.Generic;
using UnityEditor;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class EventTrigerEventInspectorHelper : ScriptableObjectSingleton<EventTrigerEventInspectorHelper>
    {
        public EventTrigerEvent skillFrameEventBase;

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
            this.skillFrameEventBase = (EventTrigerEvent)skillFrameEventBase;
            Selection.activeObject = this;
        }

        public void OnValidate()
        {
            SkillEditorWindow.Instance?.ResetView();
        }
    }
}