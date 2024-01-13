using UnityEngine;
using UnityEngine.Playables;

namespace AE_Animation_Playable.Test
{
    public class BlendTree1DTest : MonoBehaviour
    {
        private PlayableGraph graph;

        private BlendTree1D blendTree1D;
        public BlendClip1D[] blenderClip1D;

        [Range(-10, 10)]
        public float pointer;

        private void Start()
        {
            graph = PlayableGraph.Create();
            blendTree1D = new BlendTree1D(graph, blenderClip1D);

            AnimHelper.SetOutput(graph, GetComponent<Animator>(), blendTree1D);
            AnimHelper.Start(graph);
        }

        private void Update()
        {
            blendTree1D.SetValue(pointer);
        }

        private void OnDestroy()
        {
            if (enabled)
                graph.Destroy();
        }
    }
}