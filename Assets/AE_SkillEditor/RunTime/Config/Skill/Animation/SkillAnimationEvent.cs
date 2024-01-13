using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 动画帧事件
    /// </summary>
    [Serializable]
    public class SkillAnimationEvent : SkillFrameEventBase
    {
        public AnimationClip AnimationClip;     //clip
        public bool ApplyRootMotion;              //clip
        public float TransitionTime = 0.25f;    //过渡时间
#if UNITY_EDITOR
        public int DurationFrame;               //动画时间
#endif

        public override string GetName()
        {
            return AnimationClip.name;
        }

        public override object GetObject()
        {
            return AnimationClip;
        }

        public override void SetObject(object value)
        {
            AnimationClip = value as AnimationClip;
        }

        public override int GetFrameDuration(int frameRate)
        {
            return DurationFrame;
        }

        public override void SetFrameDuration(int value)
        {
            DurationFrame = value;
        }

        public override void SetName(string value)
        {
        }
    }
}