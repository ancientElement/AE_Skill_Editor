using UnityEditor;
using ARPG_AE_JOKER.SkillEditor;
using ARPG_AE_JOKER;

public class SphereAttackDetectionEventInspectorHelper : ScriptableObjectSingleton<SphereAttackDetectionEventInspectorHelper>
{
    public AttackDetectionEvent skillFrameEventBase;

    public SphereDetectionParams sphereDetectionParams;

    public void Inspector(SkillFrameEventBase skillFrameEventBase)
    {
        this.skillFrameEventBase = (AttackDetectionEvent)skillFrameEventBase;


        sphereDetectionParams = (SphereDetectionParams)this.skillFrameEventBase.DetectionParamsBase;

        Selection.activeObject = this;
    }
}
