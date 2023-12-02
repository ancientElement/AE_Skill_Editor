using UnityEditor;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SkillEffectEventDataInspectorHelper : ScriptableObjectSingleton<SkillEffectEventDataInspectorHelper>
    {
        public SkillEffectEvent skillFrameEventBase;

        public void Inspector(SkillFrameEventBase skillFrameEventBase)
        {
            this.skillFrameEventBase = (SkillEffectEvent)skillFrameEventBase;
            Selection.activeObject = this;
        }
    }
}