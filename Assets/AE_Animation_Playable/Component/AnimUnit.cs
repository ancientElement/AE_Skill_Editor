using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    /// <summary>
    /// 一个基本的动画片段
    /// </summary>
    public class AnimUnit : AnimBehaviour
    {
        private AnimationClipPlayable m_anim;

        public AnimUnit(PlayableGraph graph, AnimationClip animationClip, float enterTime = 0f) : base(graph, enterTime)
        {
            m_anim = AnimationClipPlayable.Create(graph, animationClip);
            m_adapterPlayable.AddInput(m_anim, 0, 1f);

            m_animLength = animationClip.length;

            Disable();
        }

        public override void Enable()
        {
            base.Enable();
            // 在播放开始时设置Playable的当前时间为0秒
            m_adapterPlayable.SetTime(0);
            //过 SetTime 方法，你可以设置该 Animation Clip 的当前播放时间。
            m_anim.SetTime(0);
            m_adapterPlayable.Play();
            m_anim.Play();
        }

        public override void Disable()
        {
            base.Disable();
            m_adapterPlayable.Pause();
            m_anim.Pause();
        }
    }
}