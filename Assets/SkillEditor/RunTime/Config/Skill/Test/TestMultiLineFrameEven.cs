using System;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class TestMultiLineFrameEven : SkillMultiLineFrameEventBase
    {
#if UNITY_EDITOR
        public string TrackName = "配置轨道";
#endif
        public int FrameIndex = -1;
        public int FrameDuration = -1;
        public SkillConfig skillConfig;

        public override int GetFrameDuration(int frameRate)
        {
            return FrameDuration;
        }

        public override int GetFrameIndex()
        {
            return FrameIndex;
        }

        public override string GetName()
        {
            return skillConfig.name;
        }

        public override object GetObject()
        {
            return skillConfig;
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
            throw new NotImplementedException();
        }

        public override void SetObject(object value)
        {
            skillConfig = value as SkillConfig;
        }

        public override void SetTrackName(string value)
        {
        }
    }
}