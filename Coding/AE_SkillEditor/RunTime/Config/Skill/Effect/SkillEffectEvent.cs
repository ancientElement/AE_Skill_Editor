using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    [Serializable]
    public class SkillEffectEvent : SkillMultiLineFrameEventBase
    {
#if UNITY_EDITOR
        public string TrackName = "特效";
        public int SeedRandom = 123456;
#endif
        public int FrameIndex = -1;
        public int FrameDuration = -1;
        public GameObject EffectObject;
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.one;
        public bool AutoDestroy = true;
        public float volumn = 1;

        public override int GetFrameDuration(int frameRate)
        {
            return (int)(FrameDuration);
        }

        public override int GetFrameIndex()
        {
            return FrameIndex;
        }

        public override string GetName()
        {
            return EffectObject.name;
        }

        public override object GetObject()
        {
            return EffectObject;
        }

        public override string GetTrackName()
        {
            return TrackName;
        }

        public override void SetFrameDuration(int value)
        {
            FrameDuration = value;
        }

        public override void SetFrameIndex(int value)
        {
            FrameIndex = value;
        }

        public override void SetName(string value)
        {
            throw new System.NotImplementedException();
        }

        public override void SetObject(object value)
        {
            EffectObject = value as GameObject;
        }

        public override void SetTrackName(string value)
        {
            TrackName = value;
        }
    }
}