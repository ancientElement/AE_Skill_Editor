using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AudioTrackItem : MultLineTrackItem<SkillAudioEvent>
    {
        protected override Color m_normalColor => new Color(0.706f, 0.533f, 0.020f, 1);
        protected override Color m_selectColor => new Color(0.988f, 0.749f, 0.027f, 1);

        public override void Init(ChildTrackBase<SkillAudioEvent> m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth, SimpleItemStyle simpleItemStyle = null)
        {
            this.m_ItemData = m_ParentTrack.ItemData;
            base.Init(m_ParentTrack, m_parentTrackStyle, m_ItemData, frameIndex, frameUniWidth);
        }

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

        /// <summary>
        /// 被选中事件
        /// </summary>
        public override void Select()
        {
            SkillAudioEventDataInspectorHelper.Instance.Inspector(ItemData);
        }
    }
}