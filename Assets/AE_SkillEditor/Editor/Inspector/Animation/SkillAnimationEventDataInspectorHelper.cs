using UnityEditor;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SkillAnimationEventDataInspectorHelper : ScriptableObjectSingleton<SkillAnimationEventDataInspectorHelper>
    {
        public SkillAnimationEvent skillFrameEventBase;

        public void Inspector(SkillFrameEventBase skillFrameEventBase)
        {
            this.skillFrameEventBase = (SkillAnimationEvent)skillFrameEventBase;
            Selection.activeObject = this;
        }
    }
}