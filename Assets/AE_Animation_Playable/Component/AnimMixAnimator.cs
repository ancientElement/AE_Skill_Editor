using System;
using AE_Animation_Playable;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimMixAnimator : AnimBehaviour
{
    private AnimatorControllerPlayable m_animatorControllerPlayable;
    private Animator m_animator;

    public AnimMixAnimator(PlayableGraph graph, Animator animator, float entetTime = 0f) : base(graph, entetTime)
    {
        m_animator = animator;
        m_animatorControllerPlayable = AnimatorControllerPlayable.Create(graph, animator.runtimeAnimatorController);
        m_adapterPlayable.AddInput(m_animatorControllerPlayable, 0, 1f);
    }

    public override void Enable()
    {
        base.Enable();
        m_adapterPlayable.SetTime(0);
        m_adapterPlayable.Play();
    }

    public override void Excute(Playable playable, FrameData info)
    {
        base.Excute(playable, info);
    }

    public override void Disable()
    {
        base.Disable();
        m_adapterPlayable.Pause();
    }

    public void CrossFade(string stateName, float transitionDuration)
    {
        m_animator.CrossFade(stateName, transitionDuration);
    }
    public override float GetAnimLength()
    {
        return GetAnimLength(0);
    }
    public float GetAnimLength(int layer)
    {
        return m_animator.GetCurrentAnimatorClipInfo(layer).Length;
    }
    public void SetFloat(string name, float value) { m_animator.SetFloat(name, value); }
    public void SetBool(string name, bool value) { m_animator.SetBool(name, value); }
    public void SetTrigger(string name) { m_animator.SetTrigger(name); }
}