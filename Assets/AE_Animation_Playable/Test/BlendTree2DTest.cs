using Unity.Rendering.HybridV2;
using UnityEngine;
using UnityEngine.Playables;

namespace AE_Animation_Playable.Test
{
    public class BlendTree2DTest : MonoBehaviour
    {
        private PlayableGraph graph;

        [Range(-1, 1)]
        public float x;

        [Range(-1, 1)]
        public float y;
        private BlendTree2D blenderTree2D;
        public BlendClip2D[] blenderClip2D;

        public Vector2 pointer;

        private void Start()
        {
            graph = PlayableGraph.Create();
            blenderTree2D = new BlendTree2D(graph, blenderClip2D);

            AnimHelper.SetOutput(graph, GetComponent<Animator>(), blenderTree2D);
            AnimHelper.Start(graph);
        }

        private void Update()
        {
            blenderTree2D.SetPointer(new Vector2(x,y));
        }

        private void OnDestroy()
        {
            if (enabled)
                graph.Destroy();
        }
    }
}