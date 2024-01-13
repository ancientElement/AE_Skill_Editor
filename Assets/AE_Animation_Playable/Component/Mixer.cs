using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    public class Mixer : AnimBehaviour
    {
        public int inputCount { get; private set; }
        public int currentIndex => m_currentIndex;
        public bool isTransition => m_isTransition;

        private AnimationMixerPlayable m_animMixer;

        #region 切换动画相关的变量

        private float m_timeToNext;
        private bool m_isTransition;
        private int m_targetIndex;
        private int m_currentIndex;
        private float m_currentSpeed;

        private List<int> m_declinedIndex;
        private float m_declinedSpeed;
        private float m_declinedWeight;

        #endregion 切换动画相关的变量

        public Mixer(PlayableGraph graph) : base(graph)
        {
            m_animMixer = AnimationMixerPlayable.Create(graph, 0);
            m_adapterPlayable.AddInput(m_animMixer, 0, 1f);

            m_declinedIndex = new List<int>();
            m_targetIndex = -1;
        }

        public override void AddInput(Playable playable)
        {
            base.AddInput(playable);
            m_animMixer.AddInput(playable, 0, 0f);
            inputCount++;
            if (inputCount == 1)
            {
                m_animMixer.SetInputWeight(0, 1f);
                m_currentIndex = 0;
            }
        }

        public override void Enable()
        {
            base.Enable();

            for (int i = 0; i < inputCount; i++)
            {
                m_animMixer.SetInputWeight(i, 0f);
                AnimHelper.Disable(m_animMixer.GetInput(i));
            }

            if (inputCount > 0)
            {
                AnimHelper.Enable(m_animMixer, 0);
            }
            m_animMixer.SetTime(0f);
            m_animMixer.Play();
            m_adapterPlayable.SetTime(0f);
            m_adapterPlayable.Play();

            m_animMixer.SetInputWeight(0, 1f);

            m_currentIndex = 0;
            m_targetIndex = -1;
        }

        public override void Disable()
        {
            base.Disable();
            m_animMixer.Pause();
            m_adapterPlayable.Pause();
        }

        public override void Excute(Playable playable, FrameData info)
        {
            base.Excute(playable, info);

            if (!enable) return;
            if (!m_isTransition || m_targetIndex < 0) return;

            if (m_timeToNext > 0f)
            {
                m_timeToNext -= info.deltaTime;

                m_declinedWeight = 0f;
                for (int i = 0; i < m_declinedIndex.Count; i++)
                {
                    var w = ModifyWeight(m_declinedIndex[i], -info.deltaTime * m_declinedSpeed);
                    if (w <= 0f)
                    {
                        AnimHelper.Disable(m_animMixer, m_declinedIndex[i]);
                        m_declinedIndex.Remove(m_declinedIndex[i]);
                    }
                    else
                    {
                        m_declinedWeight += w;
                    }
                }
                m_declinedWeight += ModifyWeight(m_currentIndex, -info.deltaTime * m_currentSpeed);
                SetWeight(m_targetIndex, 1f - m_declinedWeight);
                return;
            }

            m_isTransition = false;

            AnimHelper.Disable(m_animMixer, m_currentIndex);
            m_currentIndex = m_targetIndex;
            m_targetIndex = -1;
        }

        public void TransitionTo(int index)
        {
            if (m_isTransition && m_targetIndex >= 0)
            {
                if (index == m_targetIndex) return;
                if (index == m_currentIndex)
                {
                    m_currentIndex = m_targetIndex;
                }
                else if (GetWeight(m_currentIndex) > GetWeight(m_targetIndex))
                {
                    m_declinedIndex.Add(m_targetIndex);
                }
                else
                {
                    m_declinedIndex.Add(m_currentIndex);
                    m_currentIndex = m_targetIndex;
                }
            }
            else
            {
                if (index == m_currentIndex) return;
            }

            m_targetIndex = index;
            m_declinedIndex.Remove(m_targetIndex);
            AnimHelper.Enable(m_animMixer, m_targetIndex);
            m_timeToNext = GetTargetEnterTime(m_targetIndex) * (1f - GetWeight(m_targetIndex));
            m_declinedSpeed = 2f / m_timeToNext;
            m_currentSpeed = GetWeight(m_currentIndex) / m_timeToNext;
            m_isTransition = true;
        }

        public float GetWeight(int index)
        {
            if (index >= 0 && index < m_animMixer.GetInputCount())
                return m_animMixer.GetInputWeight(index);
            return 0f;
        }

        public void SetWeight(int index, float weight)
        {
            if (index >= 0 && index < m_animMixer.GetInputCount())
                m_animMixer.SetInputWeight(index, weight);
        }

        private float GetTargetEnterTime(int index)
        {
            return ((ScriptPlayable<AnimAdpter>)m_animMixer.GetInput(index)).GetBehaviour().GeteEnterTime();
        }

        private float ModifyWeight(int index, float delta)
        {
            if (index < 0 || index >= inputCount)
                return 0;
            float weight = Mathf.Clamp01(m_animMixer.GetInputWeight(index) + delta);
            m_animMixer.SetInputWeight(index, weight);
            return weight;
        }
    }
}