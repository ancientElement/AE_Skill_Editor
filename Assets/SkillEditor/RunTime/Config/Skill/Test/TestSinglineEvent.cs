using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class TestSinglineEvent : SkillFrameEventBase
    {
        public TextAsset Text;
        public int Duration = 2;

        public override int GetFrameDuration(int frameRate)
        {
            return Duration;
        }

        public override string GetName()
        {
            return Text.name;
        }

        public override object GetObject()
        {
            return Text;
        }

        public override void SetFrameDuration(int value)
        {
            Duration = value;
        }

        public override void SetName(string value)
        {
            throw new NotImplementedException();
        }

        public override void SetObject(object value)
        {
            Text = value as TextAsset;
        }
    }
}