using UnityEngine;
using UnityEngine.Playables;

namespace AE_Animation_Playable.Test
{
    public class MixerTest : MonoBehaviour
    {
        public AnimationClip[] clips;
        public bool isTransition;
        public int current;
        private PlayableGraph graph;
        private Mixer mixer;

        private void Start()
        {
            graph = PlayableGraph.Create();
            var anim1 = new AnimUnit(graph, clips[0], 0.2f);
            var anim2 = new AnimUnit(graph, clips[1], 0.2f);
            var anim3 = new AnimUnit(graph, clips[2], 0.2f);

            mixer = new Mixer(graph);

            mixer.AddInput(anim1);
            mixer.AddInput(anim2);
            mixer.AddInput(anim3);

            AnimHelper.SetOutput(graph, GetComponent<Animator>(), mixer); AnimHelper.Start(graph);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                mixer.TransitionTo(0);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                mixer.TransitionTo(1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                mixer.TransitionTo(2);
            }
        }

        private void OnDestroy()
        {
            if (enabled)
                graph.Destroy();
        }
    }
}