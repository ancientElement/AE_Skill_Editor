using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AudioChildTrack : MultiLineChildTrack<SkillAudioEvent, AudioTrackItem>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parentStyle"></param>
        /// <param name="frameWidth"></param>
        /// <param name="index"></param>
        /// <param name="DelectChildTrackAction"></param>
        public override void Init(MultiLineTrack parentTrack, SkillMultiLineTrackStyle parentStyle, float frameWidth, int index, Action<int> DelectChildTrackAction, SkillAudioEvent skillAudioEvent)
        {
            base.Init(parentTrack, parentStyle, frameWidth, index, DelectChildTrackAction, skillAudioEvent);
        }

        public override string[] GetObjectType()
        {
            return new string[] { typeof(AudioClip).FullName };
        }

        public override int CaculateObjectLength(object clip)
        {
            return (int)((clip as AudioClip).length * SkillEditorWindow.Instance.SkillConfig.FrameRate);
        }
    }
}