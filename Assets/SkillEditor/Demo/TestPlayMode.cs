using System;
using UnityEngine;
using UnityEngine.InputSystem;
using ARPG_AE_JOKER.SkillEditor;

public class TestPlayMode : MonoBehaviour
{
    [SerializeField] private Animation_Controller animation_Controller;
    [SerializeField] private Transform modelTransfrom;
    [SerializeField] private Skill_Player skill_Player;
    [SerializeField] private SkillConfig[] skillConfig;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Rigidbody body;
    [SerializeField] private AnimationClip idleClip;
    [SerializeField] private Transform[] weapons;
    [SerializeField] private SkillConfig[] katana_open;
    [SerializeField] private SkillConfig[] grateSword_open;
    [Range(0, 9.8f), SerializeField] private float grativ;

    [SerializeField, Range(1, -1)] private int useRigidBodyOrCharactorContrller;

    private void Awake()
    {
        skill_Player.Init(animation_Controller, modelTransfrom);
        animation_Controller.Init();
    }

    private Keyboard keyboard;
    private Mouse mouse;
    private int currentKey;

    private void Start()
    {
        animation_Controller.PlaySingleAnimation(idleClip);

        keyboard = Keyboard.current;
        mouse = Mouse.current;

        foreach (var item in weapons)
        {
            item.gameObject.SetActive(false);
        }

        keyboard.onTextInput += (c) =>
        {
            try
            {
                currentKey = int.Parse(c.ToString());
                //print(currentKey);
            }
            catch (Exception)
            {
            }
            if (currentKey >= 0 && currentKey < skillConfig.Length)
            {
                skill_Player.PlaySkill(skillConfig[currentKey], SkillEnd, RootMotionEvent);
            }
        };

        animation_Controller.AddAnimationEventListener("Start_Input", () =>
        {
            Debug.Log("Start_Input");
        });
    }

    private void Update()
    {
        if (mouse.leftButton.wasPressedThisFrame)
        {
            if (weapons[0].gameObject.activeSelf && katana_open.Length > 0)
            {
                if (katana_open.Length > 0)
                {
                    skill_Player.PlaySkill(katana_open[1], SkillEnd, RootMotionEvent);
                }
                weapons[0].gameObject.SetActive(false);
            }
            else if (!weapons[0].gameObject.activeSelf)
            {
                weapons[0].gameObject.SetActive(true);
                weapons[1].gameObject.SetActive(false);
                if (katana_open.Length > 0)
                {
                    skill_Player.PlaySkill(katana_open[0], SkillEnd, RootMotionEvent);
                }
            }
        }
        if (mouse.rightButton.wasPressedThisFrame)
        {
            if (weapons[1].gameObject.activeSelf && katana_open.Length > 0)
            {
                if (katana_open.Length > 0)
                {
                    skill_Player.PlaySkill(grateSword_open[1], SkillEnd, RootMotionEvent);
                }
                weapons[1].gameObject.SetActive(false);
            }
            else if (!weapons[1].gameObject.activeSelf)
            {
                weapons[0].gameObject.SetActive(false);
                weapons[1].gameObject.SetActive(true);
                if (katana_open.Length > 0)
                {
                    skill_Player.PlaySkill(grateSword_open[0], SkillEnd, RootMotionEvent);
                }
            }
        }
    }

    private void SkillEnd()
    {
        print("End");
        animation_Controller.PlaySingleAnimation(idleClip);
    }

    public void RootMotionEvent(Vector3 pos, Quaternion rot, Vector3 velocity)
    {
        pos.y -= grativ * Time.deltaTime;
        if (useRigidBodyOrCharactorContrller < 0)
            body.velocity = velocity;
        else
            characterController.Move(pos);
        modelTransfrom.transform.rotation *= rot;
    }
}