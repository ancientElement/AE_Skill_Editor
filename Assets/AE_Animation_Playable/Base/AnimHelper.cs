using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    public class AnimHelper
    {
        public static void Enable(AnimationMixerPlayable mix, int index)
        {
            Enable(mix.GetInput(index));
        }

        public static void Enable(Playable playable)
        {
            AnimAdpter adapter = GetAnimAdpter(playable);
            adapter?.Enable();
        }

        public static void Disable(AnimationMixerPlayable mix, int index)
        {
            Disable(mix.GetInput(index));
        }

        public static void Disable(Playable playable)
        {
            AnimAdpter adapter = GetAnimAdpter(playable);
            adapter?.Disable();
        }

        public static AnimAdpter GetAnimAdpter(Playable playable)
        {
            if (typeof(AnimAdpter).IsAssignableFrom(playable.GetPlayableType()))
                return ((ScriptPlayable<AnimAdpter>)playable).GetBehaviour();
            else
                return null;
        }

        public static void SetOutput(PlayableGraph graph, Animator animator, AnimBehaviour behaviour)
        {
            //添加一个根节点防止graph一播放就开始计时
            var root = new Root(graph);
            root.AddInput(behaviour);
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Anim", animator);
            //AnimationPlayableOutput.SetSourcePlayable 是 Unity 中用于设置 AnimationPlayableOutput 的源 Playable 的方法
            //这个方法允许你指定一个 Playable 作为 AnimationPlayableOutput 的输入
            output.SetSourcePlayable(root.GetAdaptrtPlayable());
        }

        public static void Start(PlayableGraph graph)
        {
            graph.Play();
            GetAnimAdpter(graph.GetOutputByType<AnimationPlayableOutput>(0).GetSourcePlayable()).Enable();
        }

        public static void Start(PlayableGraph graph, AnimBehaviour behaviour)
        {
            graph.Play();
            behaviour.Enable();
        }

        public static ComputeShader GetComputeShader(string name)
        {
            return Object.Instantiate(Resources.Load<ComputeShader>("Compute/" + name));
        }
    }
}