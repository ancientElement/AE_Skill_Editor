using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public abstract class TrackItemBase
    {
        public float frameUniWidth;//帧间距

        public virtual void Init(TrackBase m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth)
        {
        }

        public abstract void Select();

        public abstract void OnSelect();

        public abstract void UnSelect();

        public abstract void OnConfigChaged();

        public virtual void ResetView()
        { ResetRealView(frameUniWidth); }

        public virtual void ResetRealView(float frameUniWidth)
        { this.frameUniWidth = frameUniWidth; }
    }

    public class TrackItemBase<TParentTrack, TEvent> : TrackItemBase where TParentTrack : TrackBase where TEvent : SkillFrameEventBase
    {
        protected TParentTrack m_ParentTrack;//父级轨道
        private SkillTrackStyleBase m_parentTrackStyle;//父级的样式

        protected TEvent m_ItemData;//Item的数据
        public TEvent ItemData { get => m_ItemData; }//Item的数据
        protected SimpleItemStyle m_SelfStyle;//自己的样式

        public SimpleItemStyle SelfStyle
        { get { return m_SelfStyle; } }

        protected virtual Color m_normalColor => default;//正常情况颜色
        protected virtual Color m_selectColor => default; //选中情况颜色

        protected int frameIndex;//开始帧
        public int FrameIndex { get => frameIndex; }//开始帧

        public virtual void Init(TParentTrack m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth)
        {
            this.m_ParentTrack = m_ParentTrack;
            this.m_parentTrackStyle = m_parentTrackStyle;
            this.frameIndex = frameIndex;
            this.frameUniWidth = frameUniWidth;

            this.m_ItemData = m_ItemData as TEvent;
            this.m_SelfStyle = new SimpleItemStyle();

            this.m_SelfStyle.Init(m_parentTrackStyle, CaculateWidth(frameUniWidth), CaculatePosiotion(frameUniWidth), CaculateName());

            //注册常用右键事件
            RegisterAllRightButtonCLick(DeleteTrackItem, CopyTrackItem, CutTrackItem);

            //常用鼠标事件监听
            RegisterAllItemDragEvent(DragItemBegin, DragItemEnd, DragItemOut, DragItem);

            UnSelect();
        }

        #region 鼠标对Item的拖拽事件监听

        protected bool mouseDrag;
        protected float startDragPosx;
        protected int startDragFrameIndex;

        protected virtual void DragItem(MouseMoveEvent evt)
        {
            mouseDrag = true;
        }

        protected virtual void DragItemEnd(MouseUpEvent evt)
        {
            if (mouseDrag) ApplayDrag();
            mouseDrag = false;
        }

        protected virtual void DragItemOut(MouseOutEvent evt)
        {
            if (mouseDrag) ApplayDrag();
            mouseDrag = false;
        }

        protected virtual void DragItemBegin(MouseDownEvent evt)
        {
            mouseDrag = true;
            startDragFrameIndex = frameIndex;
            startDragPosx = evt.mousePosition.x;
            //显示Inspactor窗口数据
            OnSelect();
        }

        protected virtual void ApplayDrag()
        {
            Debug.Log("ApplayDrag");
        }

        #endregion 鼠标对Item的拖拽事件监听

        #region 复制粘贴剪切事件

        protected virtual void CopyTrackItem(DropdownMenuAction obj)
        {
            Debug.Log(obj.name);
        }

        protected virtual void DeleteTrackItem(DropdownMenuAction obj)
        {
            Debug.Log(obj.name);
        }

        protected virtual void CutTrackItem(DropdownMenuAction obj)
        {
            CopyTrackItem(obj);
            DeleteTrackItem(obj);
            Debug.Log(obj.name);
        }

        #endregion 复制粘贴剪切事件

        /// <summary>
        /// 计算宽度
        /// </summary>
        /// <returns></returns>
        public virtual float CaculateWidth(float frameUniWidth)
        { return -1; }

        /// <summary>
        /// 计算位置
        /// </summary>
        /// <returns></returns>
        public virtual float CaculatePosiotion(float frameUniWidth)
        { return -1; }

        /// <summary>
        /// 计算Item名字
        /// </summary>
        /// <returns></returns>
        public virtual string CaculateName()
        { return "错误"; }

        /// <summary>
        /// 注册常用右键事件
        /// </summary>
        /// <param name="DeleteTrackItem"></param>
        /// <param name="CopyTrackItem"></param>
        /// <param name="CutTrackItem"></param>
        public void RegisterAllRightButtonCLick(Action<DropdownMenuAction> DeleteTrackItem, Action<DropdownMenuAction> CopyTrackItem, Action<DropdownMenuAction> CutTrackItem)
        {
            //删除事件右键菜单
            RegsitorRightButtonClickEvent(m_SelfStyle.root, "删除", DeleteTrackItem);
            //复制事件右键菜单
            RegsitorRightButtonClickEvent(m_SelfStyle.root, "复制", CopyTrackItem);
            //剪切事件右键菜单
            RegsitorRightButtonClickEvent(m_SelfStyle.root, "剪切", CutTrackItem);
        }

        /// <summary>
        /// 注册常用鼠标事件监听
        /// </summary>
        /// <param name="MouseDown"></param>
        /// <param name="MouseUp"></param>
        /// <param name="MouseOut"></param>
        /// <param name="MouseMove"></param>
        public void RegisterAllItemDragEvent(EventCallback<MouseDownEvent> MouseDown, EventCallback<MouseUpEvent> MouseUp, EventCallback<MouseOutEvent> MouseOut, EventCallback<MouseMoveEvent> MouseMove)
        {
            //鼠标事件监听
            RegisterMouseEvent<MouseDownEvent>(m_SelfStyle.root, MouseDown);
            RegisterMouseEvent<MouseUpEvent>(m_SelfStyle.root, MouseUp);
            RegisterMouseEvent<MouseOutEvent>(m_SelfStyle.root, MouseOut);
            RegisterMouseEvent<MouseMoveEvent>(m_SelfStyle.root, MouseMove);
        }

        /// <summary>
        /// 注册右键菜单
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegsitorRightButtonClickEvent(string elementName, string name, Action<DropdownMenuAction> callback)
        {
            RegsitorRightButtonClickEvent(m_SelfStyle.FindElement<VisualElement>(elementName), name, callback);
        }

        /// <summary>
        /// 注册右键菜单
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegsitorRightButtonClickEvent(VisualElement element, string name, Action<DropdownMenuAction> callback)
        {
            element.AddOneRightMenue(name, callback);
        }

        /// <summary>
        /// 注册鼠标事件
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterMouseEvent<TEventType>(string elementName, EventCallback<TEventType> callback) where TEventType : EventBase<TEventType>, new()
        {
            RegisterMouseEvent<TEventType>(m_SelfStyle.FindElement<VisualElement>(elementName), callback);
        }

        /// <summary>
        /// 注册鼠标事件
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterMouseEvent<TEventType>(VisualElement element, EventCallback<TEventType> callback) where TEventType : EventBase<TEventType>, new()
        {
            element.RegisterCallback<TEventType>(callback);
        }

        /// <summary>
        /// 重置视图
        /// </summary>
        /// <param name="frameUniWidth"></param>
        public override void ResetRealView(float frameUniWidth)
        {
            base.ResetRealView(frameUniWidth);
            //重置视图
            m_SelfStyle.RsetView(CaculateName(), CaculatePosiotion(frameUniWidth), CaculateWidth(frameUniWidth));
        }

        public override void OnSelect()
        {
            SkillEditorWindow.Instance.UnSelectAll();
            m_SelfStyle.SetBGColor(m_selectColor);
            Select();
        }

        public override void UnSelect()
        {
            m_SelfStyle.SetBGColor(m_normalColor);
        }

        public override void OnConfigChaged()
        {
        }

        /// <summary>
        /// 被选中
        /// </summary>
        public override void Select()
        {
        }
    }
}