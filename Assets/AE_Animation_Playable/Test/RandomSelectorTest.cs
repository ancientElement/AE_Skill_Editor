using UnityEngine;
using UnityEngine.Playables;

namespace AE_Animation_Playable.Base
{
    public class RandomSelectorTest : MonoBehaviour
    {
        public bool isTransition;
        public float remianTime;

        private Mixer mixer;

        private PlayableGraph grapha;
        private RandomSelector randomSelector;

        public AnimationClip[] animationClips;
        public AnimationClip idleClip;

        private void Start()
        {
            grapha = PlayableGraph.Create();
            randomSelector = new RandomSelector(grapha,0.2f);
            var idle = new AnimUnit(grapha, idleClip, 0.2f);

            for (int i = 0; i < animationClips.Length; i++)
            {
                randomSelector.AddInput(animationClips[i], 0.2f);
            }
            mixer = new Mixer(grapha);
            mixer.AddInput(idle);
            mixer.AddInput(randomSelector);

            AnimHelper.SetOutput(grapha, GetComponent<Animator>(), mixer);

            AnimHelper.Start(grapha);
        }

        private void Update()
        {
            if (Input   .GetKeyDown(KeyCode.A) && !mixer.isTransition)
            {
                var index = randomSelector.Select();
                Debug.Log(index);
                mixer.TransitionTo(1);
            }

            isTransition = mixer.isTransition;
            remianTime = randomSelector.remainTime;

            if (!mixer.isTransition && randomSelector.remainTime == 0f && mixer.currentIndex != 0)
            {
                mixer.TransitionTo(0);
            }
        }

        private void OnDestroy()
        {
            if (enabled)
                grapha.Destroy();
        }
    }
}