using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SingleAnimationNode : AnimationNodeBase
    {
        private AnimationClipPlayable clipPlayable;

        public void Init(PlayableGraph graph, AnimationMixerPlayable mixer, AnimationClip clip, float speed, int inputPort)
        {
            clipPlayable = AnimationClipPlayable.Create(graph, clip);
            clipPlayable.SetSpeed(speed);
            this.InputPort = inputPort;
            graph.Connect(clipPlayable, 0, mixer, inputPort);
        }

        public AnimationClip GetAnimationClip()
        {
            return clipPlayable.GetAnimationClip();
        }

        public override void SetSpeed(float speed)
        {
            clipPlayable.SetSpeed(speed);
        }
    }
}