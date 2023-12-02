using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SingleLineTrackItem<TEvent> : TrackItemBase<TrackBase, TEvent> where TEvent : SkillFrameEventBase
    {
        public override void Init(TrackBase m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth)
        {
            base.Init(m_ParentTrack, m_parentTrackStyle, m_ItemData, frameIndex, frameUniWidth);
        }

        /// <summary>
        /// 复制事件右键菜单
        /// </summary>
        /// <param name="obj"></param>
        protected override void CopyTrackItem(DropdownMenuAction obj)
        {
            m_ParentTrack.tempObject = m_ItemData.GetObject();
        }

        /// <summary>
        /// 删除事件右键菜单
        /// </summary>
        /// <param name="obj"></param>
        protected override void DeleteTrackItem(DropdownMenuAction obj)
        {
            Selection.activeObject = null;
            m_ParentTrack.DelectTrackItem(frameIndex);
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        /// <param name="frameUniWidth"></param>
        public override void ResetRealView(float frameUniWidth)
        {
            base.ResetRealView(frameUniWidth);
        }

        #region TrackItem 鼠标事件监听

        protected override void DragItem(MouseMoveEvent evt)
        {
            if (mouseDrag)
            {
                float offsetPosx = evt.mousePosition.x - startDragPosx;
                int offsetFrame = Mathf.RoundToInt(offsetPosx / frameUniWidth);

                int targetFrameIndex = offsetFrame + startDragFrameIndex;
                if (targetFrameIndex < 0) return;

                bool checkDrag = false;

                if (offsetFrame > 0) //向右移动
                {
                    checkDrag = m_ParentTrack.CheckFrameIndexOnDrag(targetFrameIndex + m_ItemData.GetFrameDuration(0), startDragFrameIndex, false);
                }
                else if (offsetFrame < 0)//向左移动
                {
                    checkDrag = m_ParentTrack.CheckFrameIndexOnDrag(targetFrameIndex, startDragFrameIndex, true);
                }

                if (checkDrag)
                {
                    //更新数据
                    frameIndex = targetFrameIndex;
                    //超过轨道边界自动拓展
                    CheckFrameCount();
                    //刷新视图
                    ResetRealView(frameUniWidth);
                }
            }
        }

        protected override void ApplayDrag()
        {
            if (startDragFrameIndex != frameIndex)
            {
                m_ParentTrack.SetFrameIndex(startDragFrameIndex, frameIndex);
                if (SkillEditorInspector.Instance != null)
                    SkillEditorInspector.Instance.SetTrackItemIndex(frameIndex);
            }
        }

        #endregion TrackItem 鼠标事件监听

        /// <summary>
        /// 超过轨道边界自动拓展
        /// </summary>
        public void CheckFrameCount()
        {
            //超过轨道边界自动拓展
            if (frameIndex + m_ItemData.GetFrameDuration(0) > SkillEditorWindow.Instance.CurrentFrameCount)
            {
                SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + m_ItemData.GetFrameDuration(0);
            }
        }
    }
}