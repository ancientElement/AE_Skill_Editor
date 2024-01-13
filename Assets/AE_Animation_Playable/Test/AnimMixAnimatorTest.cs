using System.Collections;
using System.Collections.Generic;
using AE_Animation_Playable;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;

public class AnimMixAnimatorTest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private PlayableGraph graph;
    private Mixer mixer;
    private AnimUnit animUnit;
    private AnimMixAnimator animMixAnimator;
    public AnimationClip clip;


    private void Start()
    {
        graph = PlayableGraph.Create();
        mixer = new Mixer(graph);

        animUnit = new AnimUnit(graph, clip, 0.2f);
        animMixAnimator = new AnimMixAnimator(graph, animator, 0.2f);

        mixer.AddInput(animUnit);
        mixer.AddInput(animMixAnimator);

        AnimHelper.SetOutput(graph, animator, mixer);
        AnimHelper.Start(graph);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            mixer.TransitionTo(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            mixer.TransitionTo(1);
            animMixAnimator.CrossFade("RunTurn180State", 0.2f);
        }
    }

    private void OnDestroy()
    {
        if (enabled)
            graph.Destroy();
    }
}
