using UnityEngine;
using UnityEngine.Playables;

namespace AE_Animation_Playable.Test
{
    public class AnimSequencesTest : MonoBehaviour
    {
        public AnimationClip[] animationClips;
        private PlayableGraph graph;
        private AnimSequences animSequences;
        public int current;
        public int clipCount;

        private void Start()
        {
            graph = PlayableGraph.Create();
            animSequences = new AnimSequences(graph);
            for (int i = 0; i < animationClips.Length; i++)
            {
                animSequences.AddInput(animationClips[i], 0.5f);
            }
            AnimHelper.SetOutput(graph, GetComponent<Animator>(), animSequences);
            AnimHelper.Start(graph);
        }

        private void Update()
        {
            current = animSequences.currentIndex;
            clipCount = animSequences.clipCount;
        }

        private void OnDestroy() {
            if(enabled)
            graph.Destroy();
        }
    }
}