using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SkillMultiLineTrackDataBase : SkillTrackDataBase
    {
    }

    /// <summary>
    /// 轨道数据
    /// </summary>
    [Serializable]
    public class SkillMultiLineTrackDataBase<T> : SkillMultiLineTrackDataBase where T : SkillFrameEventBase
    {
        [NonSerialized, OdinSerialize]
        public List<T> FrameData = new List<T>();
    }
}