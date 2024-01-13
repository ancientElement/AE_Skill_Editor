using AE_Animation_Playable;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.AE_Animation_Playable.Test
{
    public class AnimUniTest : MonoBehaviour
    {
        private PlayableGraph graph;

        public AnimationClip clip;

        public AnimUnit anim;

        private void Start()
        {
            graph = PlayableGraph.Create();
            anim = new AnimUnit(graph, clip);

            //设置输出
            AnimHelper.SetOutput(graph, GetComponent<Animator>(), anim);

            AnimHelper.Start(graph);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (anim.enable)
                    anim.Disable();
                else
                    anim.Enable();
            }
        }

        private void OnDisable()
        {
            if (enabled)
                graph.Destroy();
        }
    }
}