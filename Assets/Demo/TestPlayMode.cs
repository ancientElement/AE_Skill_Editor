using System;
using UnityEngine;
using ARPG_AE_JOKER.SkillEditor;
using AE_Framework;

public class TestPlayMode : MonoBehaviour
{
    [SerializeField] private TestAnimationPlayer animationPlayer;
    [SerializeField] private Transform modelTransfrom;
    [SerializeField] private Skill_Player skill_Player;
    [SerializeField] private SkillConfig[] skillConfig;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private AnimationClip idleClip;
    [SerializeField] private SkillEditorSceneCamera skillEditorSceneCamera;
    [SerializeField] private GameRoot GameRoot;

    private void Awake()
    {
        GameRoot.Init();
        skill_Player.Init(animationPlayer, modelTransfrom);
        animationPlayer.Init(GetComponent<Animator>(),GetComponent<CharacterController>());
        skillEditorSceneCamera = GameObject.Find("SkillEditorSceneCamera").GetComponent<SkillEditorSceneCamera>();
        skillEditorSceneCamera.focus = GameObject.Find("CameraPos").transform;
    }


    private void Start()
    {
        animationPlayer.PlayAnimation(idleClip.name, idleClip, 0.2f);

        animationPlayer.AddAnimationEventListener("StartInput", () =>
        {
            Debug.Log("StartInput");
        });

        animationPlayer.AddAnimationEventListener("MixAnimation", () =>
        {
            Debug.Log("MixAnimation");
        });
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown((KeyCode)256 + i))
                {
                    if (skillConfig.Length > i)
                        skill_Player.PlaySkill(skillConfig[i], SkillEnd);
                }
            }
        }
    }

    private void SkillEnd()
    {
        print("End");
        animationPlayer.PlayAnimation(idleClip.name, idleClip, 0.2f);
    }
}