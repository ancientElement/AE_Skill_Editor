using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    /// <summary>
    /// 动画节点适配器
    /// </summary>
    public class AnimAdpter : PlayableBehaviour
    {
        private AnimBehaviour m_animBehaviour;

        public void Init(AnimBehaviour animBehaviour)
        {
            m_animBehaviour = animBehaviour;
        }

        public void Enable()
        {
            m_animBehaviour?.Enable();
        }

        public void Disable()
        {
            m_animBehaviour?.Disable();
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            m_animBehaviour?.Excute(playable, info);
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            m_animBehaviour?.Stop();
        }

        public float GeteEnterTime()
        {
            return m_animBehaviour.GetEnterTime();
        }
    }
}