using System;
using UnityEngine.UI;
using ARPG_AE_JOKER;
using ARPG_AE_JOKER.SkillEditor;
using System.Collections.Generic;
using UnityEngine;
namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 多行轨道Event
    /// </summary>
    [Serializable]
    public class EventTrigerEventMuil : SkillMultiLineFrameEventBase
    {
#if UNITY_EDITOR
        public string TrackName = "事件轨道";
#endif
        public int FrameIndex = -1;
        public int FrameDuration = -1;
        public List<string> eventName;
        public Color color;

        public EventTrigerEventMuil()
        {
            color = new Color(0.5f, 0.5f, 0.5f, 1);
        }

        /// <summary>
        /// 子轨道名称
        /// </summary>
        /// <returns></returns>
        public override string GetTrackName()
        {
            return TrackName;
        }
        public override void SetTrackName(string value)
        {
            TrackName = value;
        }

        /// <summary>
        /// Item的起始帧
        /// </summary>
        /// <returns></returns>
        public override int GetFrameIndex()
        {
            return FrameIndex;
        }
        public override void SetFrameIndex(int value)
        {
            FrameIndex = value;
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
            return "请修改";
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