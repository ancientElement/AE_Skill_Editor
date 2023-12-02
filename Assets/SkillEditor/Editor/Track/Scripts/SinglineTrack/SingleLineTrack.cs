using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SingleLineTrack : TrackBase
    {
    }

    public class SingleLineTrack<TEvent> : SingleLineTrack where TEvent : SkillFrameEventBase, new()
    {
        protected SkillSingLineTrackDataBase<TEvent> m_Data;//行数据
        public SkillSingLineTrackDataBase<TEvent> Data { get => m_Data; }//行数据
    }

    public class SingleLineTrack<TEvent, TChild> : SingleLineTrack<TEvent>
        where TEvent : SkillFrameEventBase, new()
        where TChild : SingleLineTrackItem<TEvent>, new()
    {
        protected string m_TrackName;//轨道名
        protected SkillSingleLineTrackStyle m_SelfStyle;//自己的样式

        protected Dictionary<int, TChild> m_TrackItems = new Dictionary<int, TChild>();//自己的Item

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth, SkillTrackDataBase m_Data, string name)
        {
            this.m_Data = (SkillSingLineTrackDataBase<TEvent>)m_Data;

            base.Init(menuParent, trackParent, frameWidth, m_Data, name);
            m_SelfStyle = new SkillSingleLineTrackStyle();
            m_SelfStyle.Init(menuParent, trackParent, name);

            //粘贴事件右键菜单
            m_SelfStyle.contentRoot.AddOneRightMenue("粘贴", PostTrackItem);
            m_SelfStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnGragUpdate);

            // 拖拽Object
            m_SelfStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnGragExit);
            m_SelfStyle.contentRoot.RegisterCallback<MouseDownEvent>(OnMouseDownEvt);

            DestroyAndReload();
        }

        /// <summary>
        /// 初始化显示
        /// </summary>
        /// <param name="frameWidth"></param>
        public override void DestroyAndReload(float frameWidth)
        {
            base.DestroyAndReload(frameWidth);
            //销毁已有的Item
            foreach (SingleLineTrackItem<TEvent> item in m_TrackItems.Values)
            {
                m_SelfStyle.DeleteItem(item.SelfStyle.root);
            }
            m_TrackItems.Clear();

            //根据数据绘制并添加
            if (SkillEditorWindow.Instance.SkillConfig != null)
            {
                foreach (KeyValuePair<int, TEvent> item in m_Data.FrameData)
                {
                    CreaetItem(item.Key, frameWidth, item.Value);
                }
            }
        }

        /// <summary>
        /// 销毁轨道显示
        /// </summary>
        public override void DestroyTrackView()
        {
            m_SelfStyle.DestroyView();
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        public override void ResetView(float frameWidth)
        {
            base.ResetView(frameWidth);
            foreach (TChild item in m_TrackItems.Values)
            {
                item.ResetRealView(frameWidth);
            }
        }

        #region Track的方法

        /// <summary>
        /// 创建单个TrackItrem
        /// </summary>
        /// <param name="frameIndex"></param>
        /// <param name="frameWidth"></param>
        /// <param name="data"></param>
        public virtual void CreaetItem(int frameIndex, float frameWidth, TEvent data)
        {
            TChild trackItem = new TChild();
            trackItem.Init(this, m_SelfStyle, data, frameIndex, frameWidth);
            m_TrackItems.Add(frameIndex, trackItem);
        }

        /// <summary>
        /// 删除TrackItem
        /// </summary>
        public override void DelectTrackItem(int index)
        {
            m_Data.FrameData.Remove(index);
            if (m_TrackItems.Remove(index, out TChild trackItem))//数据移除
            {
                m_SelfStyle.DeleteItem(trackItem.SelfStyle.root);//面板刷新
            }
            SkillEditorWindow.Instance.AutoSaveConfig();
        }

        /// <summary>
        /// 设置Item的Index
        /// </summary>
        /// <param name="oldFrame"></param>
        /// <param name="newFrame"></param>
        public override void SetFrameIndex(int oldFrame, int newFrame)
        {
            if (m_Data.FrameData.Remove(oldFrame, out TEvent data))
            {
                m_Data.FrameData.Add(newFrame, data);
                m_TrackItems.Remove(oldFrame, out TChild trackItemOld);
                m_TrackItems.Add(newFrame, trackItemOld);
                SkillEditorWindow.Instance.AutoSaveConfig();
            }
        }

        #endregion Track的方法

        #region 右键菜单

        public bool rightButtonClick;
        public float rightButtonClickPos;

        /// <summary>
        /// 鼠标事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnMouseDownEvt(MouseDownEvent evt)
        {
            if (evt.button == 1)
            {
                rightButtonClick = true;
                rightButtonClickPos = evt.localMousePosition.x;
            }
        }

        /// <summary>
        /// 粘贴事件右键菜单
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PostTrackItem(DropdownMenuAction obj)
        {
            if (rightButtonClick)
            {
                rightButtonClick = false;
                PlaceObj(tempObject, rightButtonClickPos);
            }
        }

        #endregion 右键菜单

        #region Object拖拽

        /// <summary>
        /// 拖拽事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnGragUpdate(DragUpdatedEvent evt)
        {
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;

            if (GetObjectType().Contains(objs[0].GetType().FullName))
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="evt"></param>
        private void OnGragExit(DragExitedEvent evt)
        {
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            if (objs.Length > 0)
            {
                int nextOffset = 0;
                for (int i = 0; i < objs.Length; i++)
                {
                    if (GetObjectType().Contains(objs[0].GetType().FullName))
                    {
                        PlaceObj(objs[i], evt.localMousePosition.x + nextOffset);
                        nextOffset += (int)(CaculateObjectLength(objs[i]) * base.m_FrameUniWidth);
                    }
                }
            }
        }

        #endregion Object拖拽

        /// <summary>
        /// 检测是否允许放置FrameItem
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CheckFrameIndexOnDrag(int target, int selfIndex, bool isLeft)
        {
            foreach (var item in m_Data.FrameData)
            {
                //规避自身
                if (item.Key == selfIndex) { continue; }

                //向左移动 && 在其右边 && 拖到了左边里面
                if (isLeft && selfIndex > item.Key && target < item.Key + item.Value.GetFrameDuration(0))
                {
                    return false;
                }

                //向右移动 && 在其左边 && 拖到了右边里面
                if (!isLeft && selfIndex < item.Key && target > item.Key)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 放置动画片段
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="localMousePositionX"></param>
        private void PlaceObj(object clip, float localMousePositionX)
        {
            if (clip != null)
            {
                //放置动画片段
                //判断选中帧是否可以放置
                int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexPos(localMousePositionX);

                bool canPlace = true;//是否可以放置
                int durationFrame = -1;//当前需要的帧总数
                int clipFrameCont = CaculateObjectLength(clip);//当前clip的帧数
                int nextTrackItem = -1;//下一个TrackItem的帧
                int spacing = int.MaxValue;//与下一个TrackItem的间距

                //检查选中帧是否已经有动画配置文件
                foreach (var item in m_Data.FrameData)
                {
                    //不允许在TrackItem之间
                    if (selectFrameIndex > item.Key && selectFrameIndex < item.Key + item.Value.GetFrameDuration(0))
                    {
                        canPlace = false;
                        break;
                    }
                    //找到右侧最近的TrackItem
                    if (selectFrameIndex < item.Key)
                    {
                        int tempOffset = item.Key - selectFrameIndex;
                        if (tempOffset < spacing)
                        {
                            spacing = tempOffset;
                            nextTrackItem = item.Key;
                        }
                        break;
                    }
                }

                if (canPlace)
                {
                    //如果右边有FrameItme
                    if (nextTrackItem != -1)
                    {
                        //判断剩余空间是否可以放下当前clip
                        if (spacing > clipFrameCont) durationFrame = clipFrameCont;
                        else durationFrame = spacing;
                    }
                    //右边没有FramItem
                    else
                    {
                        durationFrame = clipFrameCont;
                    }

                    TEvent newEvent = new TEvent();
                    newEvent.SetObject(clip);
                    newEvent.SetFrameDuration(durationFrame);

                    //保存数据
                    m_Data.FrameData.Add(selectFrameIndex, newEvent);
                    SkillEditorWindow.Instance.AutoSaveConfig();

                    //同步视图
                    CreaetItem(selectFrameIndex, m_FrameUniWidth, newEvent);
                }
            }
        }

        /// <summary>
        /// 获取Object类型
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetObjectType()
        {
            return default;
        }

        public override void UnSelectAll()
        {
            foreach (TChild item in m_TrackItems.Values)
            {
                item.UnSelect();
            }
        }
    }
}