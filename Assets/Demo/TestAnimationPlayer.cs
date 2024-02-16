using System;
using System.Collections.Generic;
using AE_Animation_Playable;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class TestAnimationPlayer : MonoBehaviour,ISkillAnimationPlayer
    {
        private AEAnimController m_aeanim;

        public void Init(Animator animator,CharacterController characterController)
        {
            m_aeanim = new AEAnimController(this, animator, transform, characterController);
            m_aeanim.AddAnimator("Animator",0.2f);
            m_aeanim.Start();
        }

        public void ApplayRootMotion()
        {
            m_aeanim.ApplayRootMotion();
        }

        public void PreventRootMotion()
        {
            m_aeanim.PreventRootMotion();
        }

        public void PlayAnimation(string name, AnimationClip animationClip, float enterTIme)
        {
            m_aeanim.AddAnimUnit(name, animationClip, enterTIme);
            m_aeanim.TransitionTo(name);
        }

        private void OnDestroy()
        {
            m_aeanim.Stop();
        }

        #region 动画事件

        private Dictionary<string, Action> animationEventDic = new Dictionary<string, Action>();

        public void TrigerAnimationEvent(string eventName)
        {
            if (animationEventDic.TryGetValue(eventName, out Action action))
            {
                action?.Invoke();
            }
        }

        public void AddAnimationEventListener(string name, Action action)
        {
            if (animationEventDic.TryGetValue(name, out Action _action))
            {
                _action += action;
            }
            else
            {
                animationEventDic.Add(name, action);
            }
        }

        public void RemoveAnimationEvent(string name)
        {
            animationEventDic.Remove(name);
        }

        public void RemoveAnimationEvent(string name, Action action)
        {
            if (animationEventDic.TryGetValue(name, out Action _action))
            {
                _action -= action;
            }
        }

        public void ClearAnimationEvent()
        {
            animationEventDic.Clear();
        }

        public void OnAnimatorMove()
        {
            m_aeanim.OnAnimatorMove();
        }
        #endregion 动画事件
    }
}