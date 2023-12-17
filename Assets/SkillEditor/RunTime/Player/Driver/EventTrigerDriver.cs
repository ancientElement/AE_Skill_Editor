
using ARPG_AE_JOKER.SkillEditor;
using UnityEngine;

namespace Assets.SkillEditor.RunTime.Player.Driver
{
    public class EventTrigerDriver
    {
        public static void Drive(Animation_Controller animation_Controller,SkillSingLineTrackDataBase<EventTrigerEvent> eventTrigerData, int currentFrameIndex)
        {
            if (eventTrigerData.FrameData != null)
            {
                if (eventTrigerData.FrameData.TryGetValue(currentFrameIndex, out EventTrigerEvent skillEvent))
                {
                    animation_Controller.TrigerAnimationEvent(skillEvent.eventName);
                }
            }
        }
    }
}
