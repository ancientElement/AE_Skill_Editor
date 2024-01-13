
using ARPG_AE_JOKER.SkillEditor;
using UnityEngine;

namespace Assets.SkillEditor.RunTime.Player.Driver
{
    public class EventTrigerDriver
    {
        public static void Drive(ISkillAnimationPlayer animationPlayer, SkillSingLineTrackDataBase<EventTrigerEvent> eventTrigerData, int currentFrameIndex)
        {
            if (eventTrigerData != null)
                if (eventTrigerData.FrameData != null)
                {
                    if (eventTrigerData.FrameData.TryGetValue(currentFrameIndex, out EventTrigerEvent skillEvent))
                    {
                        for (int i = 0; i < skillEvent.eventName.Count; i++)
                        {
                            animationPlayer.TrigerAnimationEvent(skillEvent.eventName[i]);
                        }
                    }
                }
        }
    }
}
