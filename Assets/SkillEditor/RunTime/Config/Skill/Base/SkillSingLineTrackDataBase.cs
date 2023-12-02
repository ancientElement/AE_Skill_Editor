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
        [NonSerialized, OdinSerialize]
        public Dictionary<int, T> FrameData = new Dictionary<int, T>();
    }
}