using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AnimationTrackItem : SingleLineTrackItem<SkillAnimationEvent>
    {
        protected override Color m_normalColor => new Color(0.333f, 0.659f, 0.596f, 1);
        protected override Color m_selectColor => new Color(0.494f, 0.980f, 0.886f, 1);

        public override void Init(TrackBase m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth, SimpleItemStyle simpleItemStyle)
        {
            base.Init(m_ParentTrack, m_parentTrackStyle, m_ItemData, frameIndex, frameUniWidth);

            //EndLine 为自定义元素绑定事件
            VisualElement animationEndLine = m_SelfStyle.FindElement<VisualElement>("EndLine");
            RegisterMouseEvent<MouseDownEvent>(animationEndLine, EndLineMouseDown);
            RegisterMouseEvent<MouseUpEvent>(animationEndLine, EndLineMouseUp);
            animationEndLine.SetCursor(MouseCursor.SplitResizeLeftRight);
            RegisterMouseEvent<MouseOutEvent>(animationEndLine, EndLineMouseOut);
            animationEndLine.SetCursor(MouseCursor.SplitResizeLeftRight);

            //绑定右键点击菜单
            RegsitorRightButtonClickEvent("Main", "RootMotionYes", (res) => { ItemData.ApplyRootMotion = true; });
            RegsitorRightButtonClickEvent("Main", "RootMotionNO", (res) => { ItemData.ApplyRootMotion = false; });

            ResetRealView(frameUniWidth);
        }

        #region EndLine 鼠标事件监听

        private bool mouseDragEndLine;

        /// <summary>
        /// 重写此方法定义拖拽
        /// </summary>
        /// <param name="evt"></param>
        protected override void DragItem(MouseMoveEvent evt)
        {
            base.DragItem(evt);
            if (mouseDragEndLine)
            {
                float offsetPosx = evt.mousePosition.x - startDragPosx;
                int offsetFrame = Mathf.RoundToInt(offsetPosx / frameUniWidth);

                int targetFrameIndex = startDragFrameIndex + offsetFrame;
                if (targetFrameIndex < 0) return;

                bool checkDrag = false;

                if (offsetFrame > 0) //向右移动
                {
                    checkDrag = m_ParentTrack.CheckFrameIndexOnDrag(targetFrameIndex, frameIndex, false);
                }
                else if (offsetFrame < 0)//向左移动
                {
                    checkDrag = m_ParentTrack.CheckFrameIndexOnDrag(targetFrameIndex, frameIndex, true);
                }

                if (checkDrag)
                {
                    ////更新数据
                    m_ItemData.DurationFrame = targetFrameIndex - frameIndex;
                    SkillEditorWindow.Instance.AutoSaveConfig();
                    ////超过轨道边界自动拓展
                    CheckFrameCount();
                    ////刷新视图
                    ResetRealView(frameUniWidth);
                }
            }
        }

        private void EndLineMouseDown(MouseDownEvent evt)
        {
            mouseDragEndLine = true;
            mouseDrag = false;
            startDragFrameIndex = frameIndex + m_ItemData.DurationFrame;
            startDragPosx = evt.mousePosition.x;
        }

        private void EndLineMouseUp(MouseUpEvent evt)
        {
            mouseDragEndLine = false;
        }

        private void EndLineMouseOut(MouseOutEvent evt)
        {
            mouseDragEndLine = false;
        }

        #region TrackItem 鼠标事件监听

        protected override void DragItemEnd(MouseUpEvent evt)
        {
            base.DragItemEnd(evt);
            mouseDragEndLine = false;
        }

        protected override void DragItemBegin(MouseDownEvent evt)
        {
            if (!mouseDragEndLine)
                base.DragItemBegin(evt);
        }

        #endregion TrackItem 鼠标事件监听

        #endregion EndLine 鼠标事件监听

        #region Caculate外观

        public override string CaculateName()
        {
            return m_ItemData.AnimationClip.name;
        }

        public override float CaculatePosiotion(float frameUniWidth)
        {
            return frameIndex * frameUniWidth;
        }

        public override float CaculateWidth(float frameUniWidth)
        {
            return m_ItemData.DurationFrame * frameUniWidth;
        }

        #endregion Caculate外观

        /// <summary>
        /// 刷新界面
        /// </summary>
        /// <param name="frameUniWidth"></param>
        public override void ResetRealView(float frameUniWidth)
        {
            base.ResetRealView(frameUniWidth);

            //结束线的位置
            int realAnimationClipFameCount = (int)(m_ItemData.AnimationClip.length * m_ItemData.AnimationClip.frameRate);
            if (realAnimationClipFameCount > m_ItemData.DurationFrame)
            {
                m_SelfStyle.FindElement<VisualElement>("OverLine").style.display = DisplayStyle.None;
            }
            else
            {
                var overLine = m_SelfStyle.FindElement<VisualElement>("OverLine");
                overLine.style.display = DisplayStyle.Flex;
                //位置
                Vector3 overLinePos = overLine.transform.position;
                overLinePos.x = realAnimationClipFameCount * frameUniWidth - 1.5f;
                overLine.transform.position = overLinePos;
            }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public override void OnConfigChaged()
        {
        }

        public override void Select()
        {
            SkillAnimationEventDataInspectorHelper.Instance.Inspector(ItemData);
        }
    }
}