//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UIElements;
//using static Codice.CM.WorkspaceServer.DataStore.WkTree.WriteWorkspaceTree;

//namespace ARPG_AE_JOKER.SkillEditor
//{
//    public class DragEndLineComponent
//    {
//        TrackItemBase<TrackBase, SkillFrameEventBase> trackItem;

//        public void Add(TrackItemBase<TrackBase, SkillFrameEventBase> trackItem)
//        {
//            this.trackItem = trackItem;
//            VisualElement animationEndLine = trackItem.SelfStyle.FindElement<VisualElement>("EndLine");
//            trackItem.RegisterMouseEvent<MouseDownEvent>(animationEndLine, EndLineMouseDown);
//            trackItem.RegisterMouseEvent<MouseUpEvent>(animationEndLine, EndLineMouseUp);
//            animationEndLine.SetCursor(MouseCursor.SplitResizeLeftRight);
//            trackItem.RegisterMouseEvent<MouseOutEvent>(animationEndLine, EndLineMouseOut);
//            animationEndLine.SetCursor(MouseCursor.SplitResizeLeftRight);
//        }

//        protected  void DragItem(MouseMoveEvent evt)
//        {
//            if (mouseDragEndLine)
//            {
//                float offsetPosx = evt.mousePosition.x - startDragPosx;
//                int offsetFrame = Mathf.RoundToInt(offsetPosx / trackItem.frameUniWidth);

//                int targetFrameIndex = startDragFrameIndex + offsetFrame;
//                if (targetFrameIndex < 0) return;

//                bool checkDrag = false;

//                if (offsetFrame > 0) //向右移动
//                {
//                    checkDrag = trackItem.Para.CheckFrameIndexOnDrag(targetFrameIndex, frameIndex, false);
//                }
//                else if (offsetFrame < 0)//向左移动
//                {
//                    checkDrag = m_ParentTrack.CheckFrameIndexOnDrag(targetFrameIndex, frameIndex, true);
//                }

//                if (checkDrag)
//                {
//                    ////更新数据
//                    m_ItemData.DurationFrame = targetFrameIndex - frameIndex;
//                    SkillEditorWindow.Instance.AutoSaveConfig();
//                    ////超过轨道边界自动拓展
//                    CheckFrameCount();
//                    ////刷新视图
//                    ResetRealView(frameUniWidth);
//                }
//            }
//        }

//        #region EndLine 鼠标事件监听

//        private bool mouseDragEndLine;
//        private bool mouseDrag;
//        private int startDragFrameIndex;
//        private float startDragPosx;

//        private void EndLineMouseDown(MouseDownEvent evt)
//        {
//            mouseDragEndLine = true;
//            mouseDrag = false;
//            startDragFrameIndex = trackItem.FrameIndex+ trackItem.ItemData.GetFrameDuration(0);
//            startDragPosx = evt.mousePosition.x;
//        }

//        private void EndLineMouseUp(MouseUpEvent evt)
//        {
//            mouseDragEndLine = false;
//        }

//        private void EndLineMouseOut(MouseOutEvent evt)
//        {
//            mouseDragEndLine = false;
//        }

//        #endregion EndLine 鼠标事件监听
//    }
//}