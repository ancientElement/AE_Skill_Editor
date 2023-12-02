using System;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public abstract class TrackBase
    {
        protected float m_FrameUniWidth;//间距

        public object tempObject;//临时的物体

        public virtual void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth, SkillTrackDataBase m_Data, string name = "")
        {
            this.m_FrameUniWidth = frameWidth;
        }

        public virtual bool CheckFrameIndexOnDrag(int target, int selfIndex, bool isLeft)
        {
            return true;
        }

        public virtual void SetFrameIndex(int oldFrame, int newFrame)
        {
        }

        public virtual int CaculateObjectLength(object clip)
        {
            return 0;
        }

        public virtual void DestroyAndReload(float frameWidth)
        { this.m_FrameUniWidth = frameWidth; }

        public virtual void DestroyAndReload()
        { DestroyAndReload(m_FrameUniWidth); }

        public virtual void ResetView(float frameWidth)
        {
            this.m_FrameUniWidth = frameWidth;
        }

        public virtual void DelectTrackItem(int index)
        { }

        public virtual void OnConfigChange()
        { }

        public virtual void OnPlay(int startFrame)
        { }

        public virtual void OnStop()
        { }

        public virtual void TickView(int currentFrameIndex)
        { }

        public virtual void DestroyTrackView()
        { }

        public virtual void UnSelectAll()
        { }

        public virtual void ClearScene() { }
    }

    public class ChildTrackBase : TrackBase
    {
        protected TrackBase m_ParentTrack;//父级轨道
        public TrackBase ParentTrack { get => m_ParentTrack; }//父级轨道
    }

    public class ChildTrackBase<TEvent> : ChildTrackBase
    {
        public TEvent m_ItemData;
        public TEvent ItemData
        { get { return m_ItemData; } }

        protected ChildTrackStyle m_SelfStyle;//自身样式

        public ChildTrackStyle SelfStyle
        { get { return m_SelfStyle; } }

        public virtual void Init(MultiLineTrack m_ParentTrack, SkillMultiLineTrackStyle parentStyle, float frameWidth, int index, Action<int> DelectChildTrackAction, TEvent m_ItemData)
        {
            this.m_FrameUniWidth = frameWidth;
        }

        public virtual void AddChildTrackItem(int frameIndex, float frameUniWidth, object audioClip)
        {
        }

        public virtual void UpdateIndex(int index, float headHeight, float itemHeight)
        {
        }

        public override void ResetView(float frameWidth)
        {
            base.ResetView(frameWidth);
        }
    }
}