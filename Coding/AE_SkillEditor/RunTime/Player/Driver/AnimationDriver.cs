using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AnimationDriver
    {
        // public static void Drive(SkillSingLineTrackDataBase<SkillAnimationEvent> AnimationData, int currentFrameIndex, RootMotionAction rootMotionAction, Animation_Controller animation_Controller)
        public static void Drive(SkillSingLineTrackDataBase<SkillAnimationEvent> AnimationData, int currentFrameIndex, AnimationPlayer animationPlayer)
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
                    animationPlayer.PlaySkill(skillEvent.AnimationClip.name, skillEvent.AnimationClip, skillEvent.TransitionTime);

                    // if (skillEvent.ApplyRootMotion)
                    // {
                    //     animation_Controller.SetRootMotion(rootMotionAction);
                    // }
                    // else
                    // {
                    //     animation_Controller.ClearRootMotionAction();
                    // }
                }
            }
        }
    }
}
