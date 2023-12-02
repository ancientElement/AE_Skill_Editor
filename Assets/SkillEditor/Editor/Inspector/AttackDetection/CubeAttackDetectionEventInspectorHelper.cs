using UnityEditor;
using ARPG_AE_JOKER;
using ARPG_AE_JOKER.SkillEditor;

public class CubeAttackDetectionEventInspectorHelper : ScriptableObjectSingleton<CubeAttackDetectionEventInspectorHelper>
{
    public AttackDetectionEvent skillFrameEventBase;

    public CubeDetectionParams cubeDetectionParams;

    public void Inspector(SkillFrameEventBase skillFrameEventBase)
    {
        this.skillFrameEventBase = (AttackDetectionEvent)skillFrameEventBase;

        cubeDetectionParams = (CubeDetectionParams)this.skillFrameEventBase.DetectionParamsBase;
        Selection.activeObject = this;
    }
}
