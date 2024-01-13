using System;
using UnityEngine;
using AE_Framework;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// RootMotion委托
    /// </summary>
    /// <param name="deltaPosition"></param>
    /// <param name="deltaRotation"></param>
    /// <param name="velocity"></param>
    public delegate void RootMotionAction(Vector3 deltaPosition, Quaternion deltaRotation, Vector3 velocity);

    public class Animation_Controller : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;

        private AnimationNodeBase previousNode;//上一个节点
        private AnimationNodeBase currentNode;//当前节点
        private int inputPort0 = 0;
        private int inputPort1 = 1;

        private Coroutine translationCoroution;

        private float speed;

        public float Speed
        {
            get => speed; set
            {
                speed = value;
                currentNode.SetSpeed(speed);
            }
        }

        public void Init()
        {
            //1.创建图
            graph = PlayableGraph.Create("Animation_Controller");
            //2.设置时间模式
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            //4.创建mixer
            mixer = AnimationMixerPlayable.Create(graph, 3);
            //6.创建OutPut
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Animation", animator);
            //7.链接mixer与output
            output.SetSourcePlayable(mixer);
        }

        //放进对象池
        private void DestoryNode(AnimationNodeBase animationNodeBase)
        {
            if (animationNodeBase != null)
            {
                graph.Disconnect(mixer, animationNodeBase.InputPort);
                animationNodeBase.PushPool();
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animationClip"></param>
        /// <param name="fixTime"></param>
        /// <param name="speed"></param>
        /// <param name="refreshAnimation"></param>
        public void PlaySingleAnimation(AnimationClip animationClip, float fixTime = 0.25f, float speed = 1, bool refreshAnimation = false)
        {
            SingleAnimationNode singleAnimationNode = null;
            if (currentNode == null)
            {
                singleAnimationNode = PoolMgr.GetObj<SingleAnimationNode>();
                if (singleAnimationNode == null)
                {
                    throw new NotImplementedException("singleAnimationNode不存在");
                }
                singleAnimationNode.Init(graph, mixer, animationClip, speed, inputPort0);
                mixer.SetInputWeight(inputPort0, 1);
            }
            else
            {
                SingleAnimationNode perNode = currentNode as SingleAnimationNode;
                //如果是相同的动画
                if (perNode != null && perNode.GetAnimationClip() == animationClip)
                {
                    if (!refreshAnimation)
                        return;
                }
                //销毁的占用的节点
                DestoryNode(previousNode);
                singleAnimationNode = PoolMgr.GetObj<SingleAnimationNode>();
                singleAnimationNode.Init(graph, mixer, animationClip, speed, inputPort1);
                previousNode = currentNode;
                StartTranslationAnimation(fixTime);
            }
            this.speed = speed;
            currentNode = singleAnimationNode;
            if (graph.IsPlaying() == false) graph.Play();
        }

        private void StartTranslationAnimation(float fixTime)
        {
            if (translationCoroution != null) StopCoroutine(translationCoroution);
            translationCoroution = StartCoroutine(TranslationAnimation(fixTime));
        }

        /// <summary>
        /// 开启过渡
        /// </summary>
        /// <param name="fixTime"></param>
        /// <param name="currentIsClipPlayable1"></param>
        /// <returns></returns>
        private IEnumerator TranslationAnimation(float fixTime)
        {
            int temp = inputPort0;
            inputPort0 = inputPort1;
            inputPort1 = temp;

            //直接过渡
            if (fixTime == 0)
            {
                mixer.SetInputWeight(inputPort1, 0);
                mixer.SetInputWeight(inputPort0, 1);
                yield break;
            }

            float currentWeight = 1;
            float speed = 1 / fixTime;

            while (currentWeight > 0)
            {
                currentWeight = Mathf.Clamp01(currentWeight - Time.deltaTime * speed);
                mixer.SetInputWeight(inputPort1, currentWeight);//减少
                mixer.SetInputWeight(inputPort0, 1 - currentWeight);//增加
                yield return null;
            }
            translationCoroution = null;
        }

        /// <summary>
        /// 播放混合动画
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="speed"></param>
        /// <param name="fixTime"></param>
        public void PlayBlendAnimation(List<AnimationClip> clips, float speed = 1, float fixTime = 0.25f)
        {
            BlendAnimationNdoe blendAnimationNdoe = PoolMgr.GetObj<BlendAnimationNdoe>();
            if (currentNode == null)
            {
                blendAnimationNdoe.Init(graph, mixer, clips, speed, inputPort0);
                mixer.SetInputWeight(inputPort0, 1);
            }
            else
            {
                DestoryNode(previousNode);
                blendAnimationNdoe.Init(graph, mixer, clips, speed, inputPort1);
                previousNode = currentNode;
                StartTranslationAnimation(fixTime);
            }
            this.speed = speed;
            currentNode = blendAnimationNdoe;
            if (graph.IsPlaying() == false) graph.Play();
        }

        /// <summary>
        /// 播放混合动画
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="speed"></param>
        /// <param name="fixTime"></param>
        public void PlayBlendAnimation(AnimationClip clip1, AnimationClip clip2, float speed = 1, float fixTime = 0.25f)
        {
            BlendAnimationNdoe blendAnimationNdoe = PoolMgr.GetObj<BlendAnimationNdoe>();
            if (currentNode == null)
            {
                blendAnimationNdoe.Init(graph, mixer, clip1, clip2, speed, inputPort0);
                mixer.SetInputWeight(inputPort0, 1);
            }
            else
            {
                DestoryNode(previousNode);
                blendAnimationNdoe.Init(graph, mixer, clip1, clip2, speed, inputPort1);
                previousNode = currentNode;
                StartTranslationAnimation(fixTime);
            }
            this.speed = speed;
            currentNode = blendAnimationNdoe;
            if (graph.IsPlaying() == false) graph.Play();
        }

        /// <summary>
        /// 设置权重
        /// </summary>
        /// <param name="weight"></param>
        public void SetBlendWeight(List<float> weight)
        {
            (currentNode as BlendAnimationNdoe).SetBlendWeight(weight);
        }

        /// <summary>
        /// 设置权重
        /// </summary>
        /// <param name="clip1Weight"></param>
        public void SetBlendWeight(float clip1Weight)
        {
            (currentNode as BlendAnimationNdoe).SetBlend1Weight(clip1Weight);
        }

        #region RootMotion
        private RootMotionAction rootMotionAction;

        private void OnAnimatorMove()
        {
            rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation, animator.velocity);
        }

        public void SetRootMotion(RootMotionAction rootMotionAction)
        {
            this.rootMotionAction = rootMotionAction;
        }

        public void ClearRootMotionAction()
        {
            this.rootMotionAction = null;
        }

        #endregion RootMotion

        #region 动画事件

        private Dictionary<string, Action> animationEventDic = new Dictionary<string, Action>();

        public void TrigerAnimationEvent(string eventName)
        {
            if (animationEventDic.TryGetValue(eventName, out Action action))
            {
                action?.Invoke();
            }
        }

        public void AddAnimationEventListener(string name, Action action)
        {
            if (animationEventDic.TryGetValue(name, out Action _action))
            {
                _action += action;
            }
            else
            {
                animationEventDic.Add(name, action);
            }
        }

        public void RemoveAnimationEvent(string name)
        {
            animationEventDic.Remove(name);
        }

        public void RemoveAnimationEvent(string name, Action action)
        {
            if (animationEventDic.TryGetValue(name, out Action _action))
            {
                _action -= action;
            }
        }

        public void ClearAnimationEvent()
        {
            animationEventDic.Clear();
        }

        #endregion 动画事件

        private void OnDestroy()
        {
            graph.Destroy();
        }
    }
}