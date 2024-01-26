using Assets.SkillEditor.RunTime.Player.Driver;
using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public interface ISkillAnimationPlayer
    {
        public void PlayAnimation(string name, AnimationClip animationClip, float enterTIme);
        public void TrigerAnimationEvent(string eventName);
        public void ApplayRootMotion();
        public void PreventRootMotion();
    }

    public class Skill_Player : MonoBehaviour
    {
        private ISkillAnimationPlayer m_animationPlayer;

        private bool isPlaying;
        public bool IsPlaying { get => isPlaying; }

        private SkillConfig skillConfig;//配置文件
        private int currentFrameIndex = -1;//当前播放帧
        private float playTotalTime;//播放时长
        private int frameRate;//帧率

        private Action SkillEndAction;
        private Transform modelTransfrom;

        private SkillSingLineTrackDataBase<SkillAnimationEvent> AnimationData;
        private SkillMultiLineTrackDataBase<SkillAudioEvent> AudioData;
        private SkillMultiLineTrackDataBase<SkillEffectEvent> EffectData;
        private SkillMultiLineTrackDataBase<AttackDetectionEvent> attackDetectionData;
        private SkillSingLineTrackDataBase<EventTrigerEvent> eventTrigerData;

        public void Init(ISkillAnimationPlayer animationPlayer, Transform modelTransfrom)
        {
            m_animationPlayer = animationPlayer;
            this.modelTransfrom = modelTransfrom;
        }

        /// <summary>
        /// 播放技能
        /// </summary>
        // public void PlaySkill(SkillConfig skillConfig, Action skillEndAction, RootMotionAction rootMotionEvent = null)
        public void PlaySkill(SkillConfig skillConfig, Action skillEndAction)
        {
            this.skillConfig = skillConfig;
            this.frameRate = skillConfig.FrameRate;
            this.currentFrameIndex = -1;
            this.playTotalTime = 0;
            this.isPlaying = true;
            // this.rootMotionAction = rootMotionEvent;
            this.SkillEndAction = skillEndAction;

            this.AnimationData = null;
            this.AudioData = null;
            this.EffectData = null;
            this.attackDetectionData = null;
            this.eventTrigerData = null;

            if (skillConfig.trackDataDic.ContainsKey("ARPG_AE_JOKER.SkillEditor.AnimationTrack"))
            {
                this.AnimationData = skillConfig.trackDataDic["ARPG_AE_JOKER.SkillEditor.AnimationTrack"] as SkillSingLineTrackDataBase<SkillAnimationEvent>;
            }
            if (skillConfig.trackDataDic.ContainsKey("ARPG_AE_JOKER.SkillEditor.AudioTrack"))
            {
                this.AudioData = skillConfig.trackDataDic["ARPG_AE_JOKER.SkillEditor.AudioTrack"] as SkillMultiLineTrackDataBase<SkillAudioEvent>;
            }
            if (skillConfig.trackDataDic.ContainsKey("ARPG_AE_JOKER.SkillEditor.EffectTrack"))
            {
                this.EffectData = skillConfig.trackDataDic["ARPG_AE_JOKER.SkillEditor.EffectTrack"] as SkillMultiLineTrackDataBase<SkillEffectEvent>;
            }
            if (skillConfig.trackDataDic.ContainsKey("ARPG_AE_JOKER.SkillEditor.AttackDetectionTrack"))
            {
                this.attackDetectionData = skillConfig.trackDataDic["ARPG_AE_JOKER.SkillEditor.AttackDetectionTrack"] as SkillMultiLineTrackDataBase<AttackDetectionEvent>;
            }
            if (skillConfig.trackDataDic.ContainsKey("ARPG_AE_JOKER.SkillEditor.EventTrigerTrack"))
            {
                this.eventTrigerData = skillConfig.trackDataDic["ARPG_AE_JOKER.SkillEditor.EventTrigerTrack"] as SkillSingLineTrackDataBase<EventTrigerEvent>;
            }
            TickSkill();
        }

        /// <summary>
        /// 播放
        /// </summary>
        private void Update()
        {
            if (isPlaying)
            {
                playTotalTime += Time.deltaTime;
                //判断在第几帧
                int tragetFrameIndex = (int)(playTotalTime * frameRate);
                //防止
                while (currentFrameIndex < tragetFrameIndex)
                {
                    //驱动技能
                    TickSkill();
                }
                if (tragetFrameIndex >= skillConfig.FrameCount)//播放完毕
                {
                    isPlaying = false;
                    skillConfig = null;
                    m_animationPlayer.PreventRootMotion();
                    SkillEndAction?.Invoke();
                }
            }
        }

        /// <summary>
        /// 实际驱动
        /// </summary>
        private void TickSkill()
        {
            currentFrameIndex += 1;

            //驱动动画
            AnimationDriver.Drive(AnimationData, currentFrameIndex, m_animationPlayer);

            //驱动音效果
            AudioDriver.Drive(AudioData, currentFrameIndex, transform.position);

            //驱动特效
            EffectDriver.Drive(EffectData, currentFrameIndex, modelTransfrom, frameRate);

            //驱动攻击检测
            AttackDetectionDriver.Driver(attackDetectionData, currentFrameIndex, modelTransfrom, frameRate);

            //事件驱动
            EventTrigerDriver.Drive(m_animationPlayer, eventTrigerData, currentFrameIndex);
        }
    }
}