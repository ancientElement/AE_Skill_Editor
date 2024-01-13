
using ARPG_AE_JOKER.SkillEditor;
using UnityEngine;

namespace Assets.SkillEditor.RunTime.Player.Driver
{
    public class EventTrigerDriver
    {
        public static void Drive(AnimationPlayer animationPlayer, SkillSingLineTrackDataBase<EventTrigerEvent> eventTrigerData, int currentFrameIndex)
        {
            if (eventTrigerData != null)
                if (eventTrigerData.FrameData != null)
                {
                    if (eventTrigerData.FrameData.TryGetValue(currentFrameIndex, out EventTrigerEvent skillEvent))
                    {
                        animationPlayer.TrigerAnimationEvent(skillEvent.eventName);
                    }
                }
        }
    }
}
