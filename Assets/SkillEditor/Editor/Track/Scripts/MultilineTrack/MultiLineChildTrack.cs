using System;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class MultiLineChildTrack<TEvent, TChild> : ChildTrackBase<TEvent>
        where TEvent : SkillMultiLineFrameEventBase
        where TChild : MultLineTrackItem<TEvent>, new()
    {
        protected int index;//轨道索引

        public int Index
        { get { return index; } }//轨道索引

        protected Button DleteButton;//删除按钮
        protected Action<int> DelectChildTrackAction;//删除轨道事件回调

        protected TChild m_TrackItem;

        public TChild TrackItem
        { get { return m_TrackItem; } }

        public override void Init(MultiLineTrack m_ParentTrack, SkillMultiLineTrackStyle parentStyle, float frameWidth, int index, Action<int> DelectChildTrackAction, TEvent m_ItemData)
        {
            base.Init(m_ParentTrack, parentStyle, frameWidth, index, DelectChildTrackAction, m_ItemData);
            this.m_ParentTrack = m_ParentTrack;
            this.DelectChildTrackAction = DelectChildTrackAction;
            this.index = index;
            this.m_ItemData = m_ItemData;

            //样式
            m_SelfStyle = parentStyle.CreateChildTrackInTheEnd(index);
            m_SelfStyle.SetName(m_ItemData.GetTrackName());

            //删除按钮
            DleteButton = m_SelfStyle.menuRoot.Q<Button>("DleteButton");
            DleteButton.clicked += DleteButtonClicked;

            //菜单名
            TextField NameField = m_SelfStyle.menuRoot.Q<TextField>("NameField");
            NameField.RegisterValueChangedCallback(NameFieldValueChange);

            //粘贴事件右键菜单
            m_SelfStyle.contentRoot.AddOneRightMenue("粘贴", PostTrackItem);

            m_SelfStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnGragUpdate);
            m_SelfStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnGragExit);
            m_SelfStyle.contentRoot.RegisterCallback<MouseDownEvent>(OnMouseDownEvt);
        }

        /// <summary>
        /// 获取Object类型
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetObjectType()
        {
            return default;
        }

        /// <summary>
        /// 销毁轨道显示
        /// </summary>
        public override void DestroyTrackView()
        {
            m_SelfStyle.DestroyView();
        }

        /// <summary>
        /// 更新轨道索引
        /// </summary>
        /// <param name="index"></param>
        /// <param name="headHeight"></param>
        /// <param name="itemHeight"></param>
        public override void UpdateIndex(int index, float headHeight, float itemHeight)
        {
            //data
            this.index = index;
            //view
            m_SelfStyle.SetPosition(index, headHeight, itemHeight);
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="oldFrame"></param>
        /// <param name="newFrame"></param>
        public override void SetFrameIndex(int oldFrame, int newFrame)
        {
            m_ItemData.SetFrameIndex(oldFrame);
            SkillEditorWindow.Instance.AutoSaveConfig();
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        public override void ResetView(float frameWidth)
        {
            if (m_TrackItem != null)
                m_TrackItem.ResetRealView(frameWidth);
        }

        #region 拖拽Object到子轨道

        /// <summary>
        /// 拖拽事件
        /// </summary>
        /// <param name="evt"></param>
        public virtual void OnGragUpdate(DragUpdatedEvent evt)
        {
            //监听用户拖拽AnimationClip
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            if (objs[0] != null && GetObjectType().Contains(objs[0].GetType().FullName))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="evt"></param>
        public virtual void OnGragExit(DragExitedEvent evt)
        {
            //监听用户拖拽AnimationClip
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            if (objs[0] != null && GetObjectType().Contains(objs[0].GetType().FullName))
            {
                PlaceClip(objs[0], evt.localMousePosition.x);
            }
        }

        #endregion 拖拽Object到子轨道

        #region 组件事件注册

        /// <summary>
        /// 删除按钮
        /// </summary>
        protected virtual void DleteButtonClicked()
        {
            DelectChildTrackAction?.Invoke(index);
        }

        /// <summary>
        /// 修改轨道名
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void NameFieldValueChange(ChangeEvent<string> evt)
        {
            if (evt.newValue != evt.previousValue)
            {
                ItemData.SetTrackName(evt.newValue);
            }
            SkillEditorWindow.Instance.AutoSaveConfig();
        }

        #endregion 组件事件注册

        #region 右键事件

        public bool rightButtonClick;
        public float rightButtonClickPos;

        /// <summary>
        /// 粘贴事件右键菜单
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void PostTrackItem(DropdownMenuAction obj)
        {
            if (rightButtonClick)
            {
                rightButtonClick = false;
                PlaceClip((m_ParentTrack.tempObject), rightButtonClickPos);
            }
        }

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

        #endregion 右键事件

        #region Track功能

        /// <summary>
        /// 放置动画片段
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="localMousePositionX"></param>
        protected virtual void PlaceClip(object clip, float localMousePositionX)
        {
            if (clip != null && m_ItemData.GetObject() == null)
            {
                int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexPos(localMousePositionX);
                AddChildTrackItem(selectFrameIndex, m_FrameUniWidth, clip);
            }
        }

        /// <summary>
        /// 子轨道添加Item
        /// </summary>
        public override void AddChildTrackItem(int frameIndex, float frameUniWidth, object audioClip)
        {
            //data
            m_ItemData.SetObject(audioClip);
            m_ItemData.SetFrameIndex(frameIndex);
            m_ItemData.SetFrameDuration(CaculateObjectLength(audioClip));
            TChild audioTrackItem = new TChild();
            audioTrackItem.Init(this, m_SelfStyle, m_ItemData, frameIndex, frameUniWidth);
            //save
            SkillEditorWindow.Instance.AutoSaveConfig();

            m_TrackItem = audioTrackItem;
        }

        #endregion Track功能

        public override void UnSelectAll()
        {
            m_TrackItem?.UnSelect();
        }
    }
}