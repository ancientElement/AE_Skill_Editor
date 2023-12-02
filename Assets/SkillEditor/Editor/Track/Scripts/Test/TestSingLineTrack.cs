using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class TestSingLineTrack : SingleLineTrack<TestSinglineEvent, TestSinglineTrackItem>
    {
        public static string TrackName = "测试单行轨道(文字轨道)";

        /// <summary>
        /// 重写此方法来定义Item数据的长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public override int CaculateObjectLength(object clip)
        {
            return 10;
        }

        /// <summary>
        /// 重写此方法来匹配对应拖拽的类型
        /// </summary>
        /// <returns></returns>
        public override string[] GetObjectType()
        {
            return new string[] { typeof(TextAsset).FullName, "UnityEditor.MonoScript" };
        }
    }

    public class TestSinglineTrackItem : SingleLineTrackItem<TestSinglineEvent>
    {
        /// <summary>
        /// 重写此属性来定义普通状态颜色
        /// </summary>
        protected override Color m_normalColor => new Color(0.333f, 0.659f, 0.596f, 1);

        /// <summary>
        /// 重写此属性来定义选中颜色
        /// </summary>
        protected override Color m_selectColor => new Color(0.494f, 0.980f, 0.886f, 1);

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
            return m_ItemData.GetFrameDuration(0) * frameUniWidth;
        }

        #endregion Caculate外观

        public override void Select()
        {
            base.Select();
        }
    }
}