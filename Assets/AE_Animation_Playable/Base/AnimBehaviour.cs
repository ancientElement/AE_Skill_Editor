using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    /// <summary>
    /// 动画节点
    /// </summary>
    public abstract class AnimBehaviour
    {
        public bool enable { get; protected set; }
        public float remainTime { get; protected set; }

        protected Playable m_adapterPlayable;//Playable就相当与一个节点
        protected float m_enterTime;
        protected float m_animLength;

        public AnimBehaviour()
        { }

        public AnimBehaviour(PlayableGraph graph, float entetTime = 0f)
        {
            m_adapterPlayable = ScriptPlayable<AnimAdpter>.Create(graph);
            ((ScriptPlayable<AnimAdpter>)m_adapterPlayable).GetBehaviour().Init(this);
            m_enterTime = entetTime;
            m_animLength = float.NaN;
        }

        public virtual void Enable()
        { enable = true; remainTime = GetAnimLength(); }

        public virtual void Disable()
        { enable = false; remainTime = 0f; }

        public virtual void Excute(Playable playable, FrameData info)
        {
            if (!enable) return;
            remainTime = remainTime > 0f ? remainTime - info.deltaTime : 0f;
        }

        public virtual void AddInput(Playable playable)
        {
        }

        public virtual void Stop()
        { }

        public void AddInput(AnimBehaviour behaviour)
        {
            AddInput(behaviour.GetAdaptrtPlayable());
        }

        public Playable GetAdaptrtPlayable()
        { return m_adapterPlayable; }

        public virtual float GetEnterTime()
        {
            return m_enterTime;
        }

        public virtual float GetAnimLength()
        {
            return m_animLength;
        }

        public virtual void SetEnterTime(float enterTIme){
            m_enterTime = enterTIme;
        }
    }
}