using System;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 帧事件基类
    /// </summary>
    [Serializable]
    public abstract class SkillFrameEventBase : IFrameEvent
    {
        public abstract string GetName();

        public abstract void SetName(string value);

        public abstract int GetFrameDuration(int frameRate);

        public abstract void SetFrameDuration(int value);

        public abstract object GetObject();

        public abstract void SetObject(object value);
    }
}