using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class BlendAnimationNdoe : AnimationNodeBase
    {
        private AnimationMixerPlayable blendMixer;
        private List<AnimationClipPlayable> blendClipPlayables = new List<AnimationClipPlayable>();

        public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, List<AnimationClip> animationClips, float speed, int inputPort)
        {
            //创建混合
            blendMixer = AnimationMixerPlayable.Create(graph, animationClips.Count);
            graph.Connect(blendMixer, 0, outputMixer, inputPort);
            this.InputPort = inputPort;
            for (int i = 0; i < animationClips.Count; i++)
            {
                CreateAndConnetAnimationClipPlayable(graph, animationClips[i], inputPort, speed);
            }
        }

        public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, AnimationClip animationClip1, AnimationClip animationClip2, float speed, int inputPort)
        {
            //创建混合
            blendMixer = AnimationMixerPlayable.Create(graph, 2);
            graph.Connect(blendMixer, 0, outputMixer, inputPort);
            this.InputPort = inputPort;
            CreateAndConnetAnimationClipPlayable(graph, animationClip1, 0, speed);
            CreateAndConnetAnimationClipPlayable(graph, animationClip2, 1, speed);
        }

        /// <summary>
        /// 创建并且添加 AnimationClipPlayable
        /// </summary>
        /// <param name="animationClip"></param>
        /// <param name="index"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        private AnimationClipPlayable CreateAndConnetAnimationClipPlayable(PlayableGraph graph, AnimationClip animationClip, int index, float speed = 1)
        {
            AnimationClipPlayable animationClipPlayable = AnimationClipPlayable.Create(graph, animationClip);
            blendClipPlayables.Add(animationClipPlayable);
            animationClipPlayable.SetSpeed(speed);
            graph.Connect(animationClipPlayable, 0, blendMixer, index);
            return animationClipPlayable;
        }

        /// <summary>
        /// 设置权重
        /// </summary>
        /// <param name="weight"></param>
        public void SetBlendWeight(List<float> weight)
        {
            for (int i = 0; i < weight.Count; i++)
            {
                blendMixer.SetInputWeight(i, weight[i]);
            }
        }

        /// <summary>
        /// 设置权重
        /// </summary>
        /// <param name="weight"></param>
        public void SetBlend1Weight(float weight)
        {
            blendMixer.SetInputWeight(0, weight);
            blendMixer.SetInputWeight(1, 1 - weight);
        }

        public override void SetSpeed(float speed)
        {
            for (int i = 0; i < blendClipPlayables.Count; i++)
            {
                blendClipPlayables[i].SetSpeed(speed);
            }
        }

        public override void PushPool()
        {
            base.PushPool();
        }
    }
}