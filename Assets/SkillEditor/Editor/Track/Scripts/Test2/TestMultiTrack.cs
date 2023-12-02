using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class TestMultiTrack : MultiLineTrack<TestMultiLineFrameEven, TestMultiChildTrack>
    {
        public static string TrackName = "测试多行轨道(SkillConfig轨道)";
    }

    public class TestMultiChildTrack : MultiLineChildTrack<TestMultiLineFrameEven, TestMultiTrackItem>
    {
        /// <summary>
        /// 重写此方法来匹配对应拖拽的类型
        /// </summary>
        /// <returns></returns>
        public override string[] GetObjectType()
        {
            return new string[] { typeof(SkillConfig).FullName };
        }

        /// <summary>
        /// 重写此方法来定义Item数据的长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public override int CaculateObjectLength(object clip)
        {
            if (m_ItemData.GetFrameDuration(0) == -1)
                return 10;
            return m_ItemData.GetFrameDuration(0);
        }
    }

    public class TestMultiTrackItem : MultLineTrackItem<TestMultiLineFrameEven>
    {
        /// <summary>
        /// 重写此属性来定义普通状态颜色
        /// </summary>
        protected override Color m_normalColor => new Color(0.706f, 0.533f, 0.020f, 1);

        /// <summary>
        /// 重写此属性来定义选中颜色
        /// </summary>
        protected override Color m_selectColor => new Color(0.988f, 0.749f, 0.027f, 1);

        /// <summary>
        /// 定义Item的 宽度 名字 和位置
        /// </summary>

        #region Caculate外观

        public override string CaculateName()
        {
            return m_ItemData.GetName();
        }

        public override float CaculatePosiotion(float frameUniWidth)
        {
            return frameIndex * frameUniWidth;
        }

        public override float CaculateWidth(float frameUniWidth)
        {
            return frameUniWidth * m_ItemData.GetFrameDuration(SkillEditorWindow.Instance.SkillConfig.FrameRate);
        }

        #endregion Caculate外观
    }
}