using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class MultiLineTrack : TrackBase
    {
    }

    public class MultiLineTrack<TEvent, TChildTrack> : MultiLineTrack
        where TEvent : SkillMultiLineFrameEventBase, new()
        where TChildTrack : ChildTrackBase<TEvent>, new()
    {
        protected SkillMultiLineTrackDataBase<TEvent> m_Data;
        public SkillMultiLineTrackDataBase<TEvent> Data { get => m_Data; }

        protected List<TChildTrack> m_ChildTrackList = new List<TChildTrack>();
        protected SkillMultiLineTrackStyle m_SelfStyle;//样式Root保存创建

        //添加子轨道按钮
        protected Button AddButton;

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth, SkillTrackDataBase m_Data, string name = "")
        {
            base.Init(menuParent, trackParent, frameWidth, m_Data, name);

            this.m_Data = (SkillMultiLineTrackDataBase<TEvent>)m_Data;

            m_SelfStyle = new SkillMultiLineTrackStyle();
            m_SelfStyle.Init(menuParent, trackParent, name);

            //添加子轨道按钮
            AddButton = m_SelfStyle.menuRoot.Q<Button>("AddButton");
            AddButton.clicked += () => { AddButtonClick(null); };

            m_SelfStyle.trackMenuItemParent.RegisterCallback<MouseDownEvent>(ItemDownEvent);
            m_SelfStyle.trackMenuItemParent.RegisterCallback<MouseMoveEvent>(ItemMoveEvent);
            m_SelfStyle.trackMenuItemParent.RegisterCallback<MouseUpEvent>(ItemUpEvent);
            m_SelfStyle.trackMenuItemParent.RegisterCallback<MouseOutEvent>(ItemOutEvent);

            DestroyAndReload(frameWidth);
        }

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <param name="mousePosY"></param>
        /// <returns></returns>
        private int GetChildIndexFromMousePos(float mousePosY)
        {
            return Mathf.Clamp(Mathf.FloorToInt(mousePosY / m_SelfStyle.itemHeight), 0, m_ChildTrackList.Count - 1);
        }

        /// <summary>
        /// DestroyAndReload
        /// </summary>
        /// <param name="frameWidth"></param>
        public override void DestroyAndReload(float frameWidth)
        {
            base.DestroyAndReload(frameWidth);

            foreach (var item in m_ChildTrackList)
            {
                item.DestroyTrackView();
            }
            m_ChildTrackList.Clear();

            for (int i = 0; i < Data.FrameData.Count; i++)
            {
                AddButtonClick(Data.FrameData[i]);
                //是否有clip
                if (Data.FrameData[i].GetObject() != null)
                {
                    AddItemInChildTrack(i, Data.FrameData[i].GetFrameIndex(), Data.FrameData[i].GetObject());
                }
            }
        }

        /// <summary>
        /// 销毁显示
        /// </summary>
        public override void DestroyTrackView()
        {
            m_SelfStyle.DestroyView();
            //销毁TrackItem
            foreach (var item in m_ChildTrackList)
            {
                item.DestroyTrackView();
            }
            m_ChildTrackList.Clear();
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        public override void ResetView(float frameWidth)
        {
            base.ResetView(frameWidth);
            foreach (TChildTrack item in m_ChildTrackList)
            {
                item.ResetView(frameWidth);
            }
        }

        #region Track 功能

        /// <summary>
        /// 添加子轨道
        /// </summary>
        protected virtual void AddButtonClick(TEvent skillAudioEvent)
        {
            //data
            TChildTrack childTrack = new TChildTrack();

            //是否是新增
            if (skillAudioEvent == null)
            {
                skillAudioEvent = new TEvent();
                Data.FrameData.Add(skillAudioEvent);
            }

            m_ChildTrackList.Add(childTrack);

            //view
            childTrack.Init(this, m_SelfStyle, m_FrameUniWidth, m_ChildTrackList.IndexOf(childTrack), DeleteChildTrack, skillAudioEvent);

            //view
            m_SelfStyle.UpdataSise(m_ChildTrackList.Count);
        }

        /// <summary>
        /// 在子轨道添加Item
        /// </summary>
        protected virtual void AddItemInChildTrack(int index, int frameIndex, object audioClip)
        {
            if (m_ChildTrackList[index] != null && Data.FrameData[index] != null)
            {
                //data
                Data.FrameData[index].SetObject(audioClip);
                Data.FrameData[index].SetFrameIndex(frameIndex);
                //view
                m_ChildTrackList[index].AddChildTrackItem(frameIndex, m_FrameUniWidth, audioClip);
            }
        }

        /// <summary>
        /// 删除子轨道
        /// </summary>
        /// <param name="index"></param>
        protected virtual void DeleteChildTrack(int index)
        {
            //view
            m_ChildTrackList[index].SelfStyle.DestroyView();

            //data
            m_ChildTrackList.RemoveAt(index);
            Data.FrameData.RemoveAt(index);

            //view update
            m_SelfStyle.UpdataSise(m_ChildTrackList.Count);
            UpdateTrackItemIndex(index);
        }

        /// <summary>
        /// 更新轨道索引
        /// </summary>
        /// <param name="index"></param>
        protected virtual void UpdateTrackItemIndex(int index)
        {
            for (int i = index; i < m_ChildTrackList.Count; i++)
            {
                m_ChildTrackList[i].UpdateIndex(i, m_SelfStyle.headHeight, m_SelfStyle.itemHeight);
            }
        }

        /// <summary>
        /// 交换轨道
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        protected virtual void ExChangeChildTrack(int index1, int index2)
        {
            if (index1 != index2)
            {
                TChildTrack tempStyle = m_ChildTrackList[index1];
                m_ChildTrackList[index1] = m_ChildTrackList[index2];
                m_ChildTrackList[index2] = tempStyle;
                UpdateTrackItemIndex(0);
                //TODO:上级轨道有实际数据变更
                TEvent tempData = Data.FrameData[index1];
                Data.FrameData[index1] = Data.FrameData[index2];
                Data.FrameData[index2] = tempData;
                SkillEditorWindow.Instance.AutoSaveConfig();
            }
        }

        #endregion Track 功能

        #region 鼠标事件监听

        private float mousePosY = -1f;
        private int lastSelect = -1;
        private int currentSelect = -1;
        private bool isDrag;

        private void ItemDownEvent(MouseDownEvent evt)
        {
            if (currentSelect != -1)
            {
            }
            //高度推导轨道
            mousePosY = evt.localMousePosition.y;
            currentSelect = GetChildIndexFromMousePos(mousePosY);
            lastSelect = currentSelect;
            isDrag = true;
        }

        private void ItemMoveEvent(MouseMoveEvent evt)
        {
            if (isDrag && currentSelect != -1)
            {
                int mouseIndex = GetChildIndexFromMousePos(evt.localMousePosition.y);
                if (mouseIndex != currentSelect)
                {
                    ExChangeChildTrack(mouseIndex, currentSelect);
                    currentSelect = mouseIndex;
                }
            }
        }

        private void ItemUpEvent(MouseUpEvent evt)
        {
            isDrag = false;
        }

        private void ItemOutEvent(MouseOutEvent evt)
        {
            //检测鼠标是否真的离开了范围
            if (!m_SelfStyle.trackMenuItemParent.contentRect.Contains(evt.localMousePosition))
            {
                isDrag = false;
            }
        }

        #endregion 鼠标事件监听

        public override void UnSelectAll()
        {
            foreach (TChildTrack item in m_ChildTrackList)
            {
                item.UnSelectAll();
            }
        }
    }
}