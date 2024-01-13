using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 多行轨道Event
    /// </summary>
    [Serializable]
    public class AttackDetectionEvent : SkillMultiLineFrameEventBase
    {
#if UNITY_EDITOR
        public string TrackName = "攻击检测轨道";
#endif
        public int FrameIndex = -1;
        public int FrameDuration = -1;

        public Vector3 Position;
        public Vector3 Rotation;

        public DetectionParamsBase DetectionParamsBase;

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
            switch (DetectionParamsBase.GetType().FullName)
            {
                case "ARPG_AE_JOKER.SkillEditor.SphereDetectionParams":
                    return "球形检测";
                case "ARPG_AE_JOKER.SkillEditor.CubeDetectionParams":
                    return "Cube检测";
                case "ARPG_AE_JOKER.SkillEditor.RingLikeDetectionParams":
                    return "扇形检测";
            }
            return "请修改";
        }
        public override void SetName(string value)
        {
            TrackName = value;
        }

        /// <summary>
        /// 获取Object
        /// </summary>
        /// <returns></returns>
        public override object GetObject()
        {
            return DetectionParamsBase;
        }
        public override void SetObject(object value)
        {
            DetectionParamsBase = (DetectionParamsBase)value;
        }
    }
}