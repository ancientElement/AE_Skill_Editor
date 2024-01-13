using AE_Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 单行轨道Event
    /// </summary>
    [Serializable]
    public class EventTrigerEvent : SkillFrameEventBase
    {
        public int FrameDuration = -1;
        public List<string> eventName;
        public Color color;

        public EventTrigerEvent()
        {
            eventName = new List<string>();
            color = new Color(0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// item的持续帧
        /// </summary>
        /// <param name="frameRate"></param>
        /// <returns></returns>
        public override int GetFrameDuration(int frameRate)
        {
            return FrameDuration;
        }
        public override void SetFrameDuration(int value)
        {
            FrameDuration = value;
        }

        /// <summary>
        /// Object的name
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            if (eventName.Count == 0) { return "请命名"; }
            return eventName[0];
        }
        public override void SetName(string value)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取Object
        /// </summary>
        /// <returns></returns>
        public override object GetObject()
        {
            return eventName;
        }

        public override void SetObject(object value)
        {
            eventName = value as List<string>;
        }
    }
}
