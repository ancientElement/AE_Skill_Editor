using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    public class AnimSelector : AnimBehaviour
    {
        public int currentIndex { get; protected set; }
        public int clipCount { get; protected set; }
        protected AnimationMixerPlayable m_mixer;
        protected List<float> m_enterTimes;
        protected List<float> m_clipLengths;

        public AnimSelector(PlayableGraph playable, float enterTime) : base(playable, enterTime)
        {
            m_mixer = AnimationMixerPlayable.Create(playable);
            m_adapterPlayable.AddInput(m_mixer, 0, 1f);

            currentIndex = -1;
            m_enterTimes = new List<float>();
            m_clipLengths = new List<float>();
        }

        public override void Enable()
        {
            base.Enable();

            if (currentIndex < 0) return;
            m_mixer.SetInputWeight(currentIndex, 1f);
            AnimHelper.Enable(m_mixer, currentIndex);
            m_mixer.SetTime(0f);
            m_mixer.Play();
            m_adapterPlayable.SetTime(0f);
            m_adapterPlayable.Play();
        }

        public override void Disable()
        {
            base.Disable();
            if (currentIndex < 0 || currentIndex >= clipCount) return;
            for (int i = 0; i < clipCount; i++)
            {
                m_mixer.SetInputWeight(i, 0f);
            }
            AnimHelper.Disable(m_mixer, currentIndex);
            m_adapterPlayable.Pause();
            m_mixer.Pause();
            currentIndex = -1;
        }

        public override void AddInput(Playable playable)
        {
            m_mixer.AddInput(playable, 0);
            clipCount++;
        }

        public void AddInput(AnimationClip animationClip, float enterTime)
        {
            AddInput(new AnimUnit(m_adapterPlayable.GetGraph(), animationClip, enterTime));
            m_clipLengths.Add(animationClip.length);
            m_enterTimes.Add(enterTime);
        }

        public virtual int Select()
        {
            return currentIndex;
        }

        public virtual void Select(int index)
        {
            currentIndex = index;
        }

        public override float GetEnterTime()
        {
            if (currentIndex >= 0 && currentIndex < m_enterTimes.Count)
                return m_enterTimes[currentIndex];
            else
                return 0f;
        }

        public override float GetAnimLength()
        {
            if (currentIndex >= 0 && currentIndex < m_clipLengths.Count)
                return m_clipLengths[currentIndex];
            else
                return 0f;
        }
    }
}