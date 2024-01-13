using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    [Serializable]
    public class SkillAudioEvent : SkillMultiLineFrameEventBase
    {
#if UNITY_EDITOR
        public string TrackName = "音效";
#endif
        public int FrameIndex = -1;
        public AudioClip AudioClip;
        public float volumn = 1;

        public override int GetFrameIndex()
        {
            return FrameIndex;
        }

        public override void SetFrameIndex(int value)
        {
            FrameIndex = value;
        }

        public override string GetTrackName()
        {
            return TrackName;
        }

        public override void SetTrackName(string value)
        {
            TrackName = value;
        }

        public override int GetFrameDuration(int frameRate)
        {
            return (int)(AudioClip.length * frameRate);
        }

        public override void SetFrameDuration(int value)
        {
        }

        public override string GetName()
        {
            return AudioClip.name;
        }

        public override void SetName(string value)
        {
            throw new System.NotImplementedException();
        }

        public override object GetObject()
        {
            return AudioClip;
        }

        public override void SetObject(object value)
        {
            AudioClip = value as AudioClip;
        }
    }
}