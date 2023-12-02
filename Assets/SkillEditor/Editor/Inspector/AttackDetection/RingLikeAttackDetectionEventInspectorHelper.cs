using UnityEditor;
using ARPG_AE_JOKER.SkillEditor;
using ARPG_AE_JOKER;
using UnityEngine;

public class RingLikeAttackDetectionEventInspectorHelper : ScriptableObjectSingleton<RingLikeAttackDetectionEventInspectorHelper>
{
    public AttackDetectionEvent skillFrameEventBase;

    public RingLikeDetectionParams ringLikeDetectionParams;

    public void Inspector(SkillFrameEventBase skillFrameEventBase)
    {
        this.skillFrameEventBase = (AttackDetectionEvent)skillFrameEventBase;

        ringLikeDetectionParams = (RingLikeDetectionParams)this.skillFrameEventBase.DetectionParamsBase;
        Selection.activeObject = this;
    }
}
