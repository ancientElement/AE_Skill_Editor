using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AnimationDriver
    {
        public static void Drive(SkillSingLineTrackDataBase<SkillAnimationEvent> AnimationData, int currentFrameIndex, ISkillAnimationPlayer animationPlayer)
        {
            if (AnimationData != null)
            {
                if (animationPlayer != null && AnimationData.FrameData.TryGetValue(currentFrameIndex, out SkillAnimationEvent skillEvent))
                {
                    if (skillEvent.ApplyRootMotion)
                    {
                        animationPlayer.ApplayRootMotion();
                    }
                    else
                    {
                        animationPlayer.PreventRootMotion();
                    }
                    animationPlayer.PlayAnimation(skillEvent.AnimationClip.name, skillEvent.AnimationClip, skillEvent.TransitionTime);
                }
            }
        }
    }
}
