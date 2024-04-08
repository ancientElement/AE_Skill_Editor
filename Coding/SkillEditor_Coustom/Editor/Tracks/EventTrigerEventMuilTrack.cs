using ARPG_AE_JOKER.SkillEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 多行轨道
    /// </summary>
    public class EventTrigerEventMuilTrack : MultiLineTrack<EventTrigerEventMuil, EventTrigerEventMuilChildTrack>
    {
        public static string TrackName = "事件轨道";
    }

    /// <summary>
    /// 多行轨道子轨道
    /// </summary>
    public class EventTrigerEventMuilChildTrack : MultiLineChildTrack<EventTrigerEventMuil, EventTrigerEventMuilTrackItem>
    {
        public override void Init(MultiLineTrack m_ParentTrack, SkillMultiLineTrackStyle parentStyle, float frameWidth, int index, Action<int> DelectChildTrackAction, EventTrigerEventMuil m_ItemData)
        {
            base.Init(m_ParentTrack, parentStyle, frameWidth, index, DelectChildTrackAction, m_ItemData);
            m_SelfStyle.contentRoot.AddOneRightMenue("轨道_添加事件", (res) => { PlaceClip(new List<string>() { "请命名" }, rightButtonClickPos); });
        }

        /// <summary>
        /// 重写此方法来定义Item数据的长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public override int CaculateObjectLength(object clip)
        {
            return 1;
        }

        /// <summary>
        /// 重写此方法来匹配对应拖拽的类型
        /// </summary>
        /// <returns></returns>
        public override string[] GetObjectType()
        {
            return new string[] { typeof(string).FullName };
        }
    }

    /// <summary>
    /// 多行轨道子轨道的单个Item
    /// </summary>
    public class EventTrigerEventMuilTrackItem : MultLineTrackItem<EventTrigerEventMuil>
    {
        /// <summary>
        /// 重写此属性来定义普通状态颜色
        /// </summary>
        protected override Color m_normalColor => m_ItemData.color;
        /// <summary>
        /// 重写此属性来定义选中颜色
        /// </summary>
        protected override Color m_selectColor => new Color(0.5717925f, 0.9811321f, 0.3831968f, 1f);

        public override void Init(ChildTrackBase<EventTrigerEventMuil> m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth, SimpleItemStyle simpleItemStyle = null)
        {
            base.Init(m_ParentTrack, m_parentTrackStyle, m_ItemData, frameIndex, frameUniWidth, new EventItemStyle());
            RegsitorRightButtonClickEvent(m_SelfStyle.root, "事件_添加事件", (obj) =>
            {
                List<string> eventNames = m_ItemData.GetObject() as List<string>;
                eventNames.Add("请命名");
            });
        }

        /// <summary>
        /// 定义Item的 宽度 名字 和位置
        /// </summary>
        #region Caculate外观

        public override string CaculateName()
        {
            return m_ItemData.GetName().Substring(0, 3);
        }

        public override float CaculatePosiotion(float frameUniWidth)
        {
            return frameIndex * frameUniWidth - 10f;//向前偏移一半
        }

        public override float CaculateWidth(float frameUniWidth)
        {
            //return m_ItemData.GetFrameDuration(0) * frameUniWidth;
            return 20;
        }

        #endregion Caculate外观

        /// <summary>
        /// 选中
        /// </summary> 
        public override void Select()
        {
            EventTrigerEventMuilInspectorHelper.Instance.Inspector(ItemData);
        }
    }
}