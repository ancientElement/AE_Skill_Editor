using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    /// <summary>
    /// 根节点
    /// 防止未播放就开始计时
    /// </summary>
    public class Root : AnimBehaviour
    {
        public Root(PlayableGraph playableGraph) : base(playableGraph)
        {
        }

        public override void Enable()
        {
            base.Enable();
            for (int i = 0; i < m_adapterPlayable.GetInputCount(); i++)
            {
                AnimHelper.Enable(m_adapterPlayable.GetInput(i));
            }
            m_adapterPlayable.SetTime(0);
            m_adapterPlayable.Play();
        }

        public override void Disable()
        {
            base.Disable();
            for (int i = 0; i < m_adapterPlayable.GetInputCount(); i++)
            {
                AnimHelper.Disable(m_adapterPlayable.GetInput(i));
            }
            m_adapterPlayable.Pause();
        }

        public override void AddInput(Playable playable)
        {
            m_adapterPlayable.AddInput(playable, 0, 1f);
        }
    }
}