using UnityEditor;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SkillAudioEventDataInspectorHelper : ScriptableObjectSingleton<SkillAudioEventDataInspectorHelper>
    {
        public SkillAudioEvent skillFrameEventBase;

        public void Inspector(SkillFrameEventBase skillFrameEventBase)
        {
            this.skillFrameEventBase = (SkillAudioEvent)skillFrameEventBase;
            Selection.activeObject = this;
        }
    }
}