using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AnimationTrack : SingleLineTrack<SkillAnimationEvent, AnimationTrackItem>
    {
        public static string TrackName = "动画轨道";

        public static Action<Vector3> PosAction;

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth, SkillTrackDataBase m_Data, string name)
        {
            //数据赋值
            base.Init(menuParent, trackParent, frameWidth, m_Data, name);
        }

        public override int CaculateObjectLength(object clip)
        {
            return (int)((clip as AnimationClip).length * (clip as AnimationClip).frameRate);
        }

        public override string[] GetObjectType()
        {
            return new string[] { typeof(AnimationClip).FullName };
        }

        /// <summary>
        /// 当配置改变时
        /// </summary>
        public override void OnConfigChange()
        {
            foreach (AnimationTrackItem item in m_TrackItems.Values)
            {
                item.OnConfigChaged();
            }
        }

        #region 预览

        /// <summary>
        /// 得到根位移
        /// </summary>
        /// <param name="currentFrameIndex"></param>
        /// <param name="isRecover"></param>
        /// <returns></returns>
        public Vector3 GetPositionFromV3(int currentFrameIndex, bool isRecover = false)
        {
            GameObject previewGameObject = SkillEditorWindow.Instance.CurrentPreviewGameObject;
            Animator animator = previewGameObject.GetComponent<Animator>();
            //根据帧数找到当前播放动画
            Dictionary<int, SkillAnimationEvent> frameData = m_Data.FrameData;

            Vector3 rootMositionTotalPosition = Vector3.zero;
            // 利用有序字典数据结构来达到有序计算的目的
            SortedDictionary<int, SkillAnimationEvent> frameDataSortedDic = new SortedDictionary<int, SkillAnimationEvent>(frameData);
            int[] keys = frameDataSortedDic.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                int key = keys[i];
                SkillAnimationEvent animationEvent = frameDataSortedDic[key];
                // 只考虑根运动配置的动画
                if (animationEvent.ApplyRootMotion == false) continue;
                int nextKeyFrame = 0;
                if (i + 1 < keys.Length) nextKeyFrame = keys[i + 1];
                // 最后一个动画 下一个关键帧计算采用整个技能的帧长度
                else nextKeyFrame = SkillEditorWindow.Instance.SkillConfig.FrameCount;

                bool isBreak = false;
                if (nextKeyFrame > currentFrameIndex)
                {
                    nextKeyFrame = currentFrameIndex;
                    isBreak = true;
                }

                // 持续帧数 = 下一个动画的帧数 - 这个动画的开始时间
                int durationFrameCount = nextKeyFrame - key;
                if (durationFrameCount > 0)
                {
                    // 动画资源总总帧数
                    float clipFrameCount = animationEvent.AnimationClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRate;
                    // 计算总的播放进度
                    float totalProgress = durationFrameCount / clipFrameCount;
                    // 播放次数
                    int playTimes = 0;
                    // 最终一次不完整的播放，也就是进度<1
                    float lastProgress = 0;
                    // 只有循环动画才需要多次采样
                    if (animationEvent.AnimationClip.isLooping)
                    {
                        playTimes = (int)totalProgress;
                        lastProgress = totalProgress - (int)totalProgress;
                    }
                    else
                    {
                        // 不循环的动画，播放进度>1也等于1,
                        if (totalProgress >= 1)
                        {
                            playTimes = 1;
                            lastProgress = 0;
                        }
                        else
                        {
                            lastProgress = totalProgress - (int)totalProgress;
                        }
                    }
                    animator.applyRootMotion = true;
                    // 完整播放部分的采样
                    if (playTimes >= 1)
                    {
                        animationEvent.AnimationClip.SampleAnimation(previewGameObject, animationEvent.AnimationClip.length);
                        Vector3 pos = previewGameObject.transform.position;
                        rootMositionTotalPosition += pos * playTimes;
                    }
                    // 不完整的部分采样
                    if (lastProgress > 0)
                    {
                        animationEvent.AnimationClip.SampleAnimation(previewGameObject, lastProgress * animationEvent.AnimationClip.length);
                        Vector3 pos = previewGameObject.transform.position;
                        rootMositionTotalPosition += pos;
                    }
                }
                if (isBreak) break;
            }
            if (isRecover)
            {
                UdpatePosture(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
            }

            PosAction?.Invoke(rootMositionTotalPosition);

            return rootMositionTotalPosition;
        }

        /// <summary>
        /// 更新姿势
        /// </summary>
        /// <param name="currentFrameIndex"></param>
        public void UdpatePosture(int currentFrameIndex)
        {
            GameObject previewGameObject = SkillEditorWindow.Instance.CurrentPreviewGameObject;
            Animator animator = previewGameObject.GetComponent<Animator>();
            //根据帧数找到当前播放动画
            Dictionary<int, SkillAnimationEvent> frameData = m_Data.FrameData;

            //找到左边距离最小的
            int currenOoffset = int.MaxValue;
            int animationEventIndex = -1;
            foreach (var item in frameData)
            {
                int tempOffset = currentFrameIndex - item.Key;
                if (tempOffset > 0 && tempOffset < currenOoffset)
                {
                    currenOoffset = tempOffset;
                    animationEventIndex = item.Key;
                }
            }
            if (animationEventIndex != -1)
            {
                SkillAnimationEvent animationEvent = frameData[animationEventIndex];
                //动画帧数
                float clipFrameCount = animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate;
                //动画播放进度
                float progress = currenOoffset / clipFrameCount;
                //播放动画
                //循环动画
                if (progress > 1 && animationEvent.AnimationClip.isLooping)
                {
                    progress -= (int)progress;
                }
                animator.applyRootMotion = animationEvent.ApplyRootMotion;
                animationEvent.AnimationClip.SampleAnimation(previewGameObject, progress * animationEvent.AnimationClip.length);
            }
        }

        /// <summary>
        /// 驱动动画显示
        /// </summary>
        public override void TickView(int currentFrameIndex)
        {
            GameObject previewGameObject = SkillEditorWindow.Instance.CurrentPreviewGameObject;

            previewGameObject.transform.position = GetPositionFromV3(currentFrameIndex, true);
        }

        #endregion 预览
    }
}