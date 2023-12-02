using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class MultLineTrackItem<TEvent> : TrackItemBase<ChildTrackBase<TEvent>, TEvent> where TEvent : SkillMultiLineFrameEventBase
    {
        public override void Init(ChildTrackBase<TEvent> m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth)
        {
            base.Init(m_ParentTrack, m_parentTrackStyle, m_ItemData, frameIndex, frameUniWidth);
        }

        /// <summary>
        /// 复制事件右键菜单
        /// </summary>
        /// <param name="obj"></param>
        protected override void CopyTrackItem(DropdownMenuAction obj)
        {
            m_ParentTrack.ParentTrack.tempObject = m_ItemData.GetObject();
        }

        /// <summary>
        /// 删除事件右键菜单
        /// </summary>
        /// <param name="obj"></param>
        protected override void DeleteTrackItem(DropdownMenuAction obj)
        {
            Selection.activeObject = null;
            m_ItemData.SetObject(null);
            m_SelfStyle.Destory();
        }

        #region TrackItem 鼠标事件监听

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="evt"></param>
        protected override void DragItem(MouseMoveEvent evt)
        {
            if (mouseDrag)
            {
                float offsetPosx = evt.mousePosition.x - startDragPosx;
                int offsetFrame = Mathf.RoundToInt(offsetPosx / frameUniWidth);

                int targetFrameIndex = offsetFrame + startDragFrameIndex;
                if (targetFrameIndex < 0 || targetFrameIndex + m_ItemData.GetFrameDuration(SkillEditorWindow.Instance.SkillConfig.FrameRate) > SkillEditorWindow.Instance.SkillConfig.FrameCount) return;
                //更新数据
                frameIndex = targetFrameIndex;
                //刷新视图
                ResetRealView(frameUniWidth);
            }
        }

        /// <summary>
        /// 拖拽成功
        /// </summary>
        protected override void ApplayDrag()
        {
            if (startDragFrameIndex != frameIndex)
            {
                m_ParentTrack.SetFrameIndex(frameIndex, 0);
            }
        }

        #endregion TrackItem 鼠标事件监听

        public override void OnSelect()
        {
            base.OnSelect();
        }
    }
}