using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AnimationDriver
    {
        public static void Drive(SkillSingLineTrackDataBase<SkillAnimationEvent> AnimationData, int currentFrameIndex, RootMotionAction rootMotionAction, Animation_Controller animation_Controller)
        {
            if (AnimationData != null)
            {
                if (animation_Controller != null && AnimationData.FrameData.TryGetValue(currentFrameIndex, out SkillAnimationEvent skillEvent))
                {
                    animation_Controller.PlaySingleAnimation(skillEvent.AnimationClip, skillEvent.TransitionTime, 1, true);

                    if (skillEvent.ApplyRootMotion)
                    {
                        animation_Controller.SetRootMotion(rootMotionAction);
                    }
                    else
                    {
                        animation_Controller.ClearRootMotionAction();
                    }
                }
            }
        }
    }
}
