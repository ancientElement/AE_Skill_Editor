using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SkillSingLineTrackDataBase : SkillTrackDataBase
    {
    }

    /// <summary>
    /// 轨道数据
    /// </summary>
    [Serializable]
    public class SkillSingLineTrackDataBase<T> : SkillSingLineTrackDataBase where T : SkillFrameEventBase
    {
        /// <summary>
        /// 轨道基数据
        /// </summary>
        /// <typeparam name="int">起始帧</typeparam>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns></returns>
        [NonSerialized, OdinSerialize]
        public Dictionary<int, T> FrameData = new Dictionary<int, T>();
    }
}