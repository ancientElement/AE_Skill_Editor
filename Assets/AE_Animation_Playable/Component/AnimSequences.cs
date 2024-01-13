using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    public class AnimSequences : AnimBehaviour
    {
        public int currentIndex { get; protected set; }
        public int clipCount { get; protected set; }
        protected Mixer m_mixer;
        protected List<float> m_enterTimes;
        protected List<float> m_clipLengths;
        protected float m_totalLength;
        protected float timer;
        public bool loop;

        public AnimSequences(PlayableGraph playable) : base(playable)
        {
            m_mixer = new Mixer(playable);
            m_adapterPlayable.AddInput(m_mixer.GetAdaptrtPlayable(), 0, 1f);

            currentIndex = -1;
            m_enterTimes = new List<float>();
            m_clipLengths = new List<float>();
        }

        public override void Enable()
        {
            base.Enable();
            currentIndex = 0;
            m_mixer.Enable();
        }

        public override void Disable()
        {
            base.Disable();
            currentIndex = 0;
            m_mixer.Disable();
        }

        public override void Excute(Playable playable, FrameData info)
        {
            base.Excute(playable, info);
            if (currentIndex < 0 || currentIndex >= clipCount) return;

            timer += info.deltaTime;
            if (timer >= m_clipLengths[currentIndex])
            {
                currentIndex++;

                if (currentIndex >= clipCount)
                {
                    if (loop) currentIndex = 0;
                    else return;
                }

                timer = 0;
                m_mixer.TransitionTo(currentIndex);
            }
        }

        public override void AddInput(Playable playable)
        {
            m_mixer.AddInput(playable);
            clipCount++;
        }

        public void AddInput(AnimationClip animationClip, float enterTime)
        {
            AddInput(new AnimUnit(m_adapterPlayable.GetGraph(), animationClip, enterTime));
            m_clipLengths.Add(animationClip.length);
            m_totalLength += animationClip.length;
            m_enterTimes.Add(enterTime);
        }

        public override float GetEnterTime()
        {
            if (currentIndex >= 0 && currentIndex < m_enterTimes.Count)
                return m_enterTimes[0];
            else
                return 0f;
        }

        public override float GetAnimLength()
        {
            if (currentIndex >= 0 && currentIndex < m_clipLengths.Count)
                return m_totalLength;
            else
                return 0f;
        }
    }
}