using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class EffectTrackItem : MultLineTrackItem<SkillEffectEvent>
    {
        protected override Color m_selectColor => new Color(0.498f, 0.839f, 0.988f, 1);
        protected override Color m_normalColor => new Color(0.388f, 0.651f, 0.769f, 1);

        public override void Init(ChildTrackBase<SkillEffectEvent> m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth, SimpleItemStyle simpleItemStyle = null)
        {
            base.Init(m_ParentTrack, m_parentTrackStyle, m_ItemData, frameIndex, frameUniWidth);


            RegsitorRightButtonClickEvent("Main", "对准位置", (res) => { ApplyTransfromButtonClick(); });

            //EndLine
            VisualElement animationEndLine = m_SelfStyle.FindElement<VisualElement>("EndLine");
            RegisterMouseEvent<MouseDownEvent>(animationEndLine, EndLineMouseDown);
            RegisterMouseEvent<MouseUpEvent>(animationEndLine, EndLineMouseUp);
            RegisterMouseEvent<MouseOutEvent>(animationEndLine, EndLineMouseOut);
            animationEndLine.SetCursor(MouseCursor.SplitResizeLeftRight);

            ResetRealView(frameUniWidth);

            AnimationTrack.PosAction -= PosAction;
            AnimationTrack.PosAction += PosAction;
        }


        Vector3 animationPos = Vector3.zero;

        private void PosAction(Vector3 obj)
        {
            animationPos = obj;
        }

        public void DeleteTrackItemAction()
        {
            DeleteTrackItem(null);
        }

        #region EndLine 鼠标事件监听

        private bool mouseDragEndLine;

        private void EndLineMouseDown(MouseDownEvent evt)
        {
            mouseDragEndLine = true;
            mouseDrag = false;
            startDragFrameIndex = frameIndex + m_ItemData.GetFrameDuration(0);
            startDragPosx = evt.mousePosition.x;
        }

        private void EndLineMouseUp(MouseUpEvent evt)
        {
            mouseDragEndLine = false;
        }

        private void EndLineMouseOut(MouseOutEvent evt)
        {
            mouseDragEndLine = false;
        }

        protected override void DragItem(MouseMoveEvent evt)
        {
            base.DragItem(evt);
            if (mouseDragEndLine)
            {
                float offsetPosx = evt.mousePosition.x - startDragPosx;
                int offsetFrame = Mathf.RoundToInt(offsetPosx / frameUniWidth);

                int targetFrameIndex = startDragFrameIndex + offsetFrame;
                if (targetFrameIndex < 0) return;
                bool checkDrag = true;

                if (checkDrag)
                {
                    ////更新数据
                    m_ItemData.SetFrameDuration(targetFrameIndex - frameIndex);
                    SkillEditorWindow.Instance.AutoSaveConfig();
                    ////刷新视图
                    ResetRealView(frameUniWidth);
                }
            }
        }

        protected override void DragItemEnd(MouseUpEvent evt)
        {
            base.DragItemEnd(evt);
            mouseDragEndLine = false;
        }

        protected override void DragItemBegin(MouseDownEvent evt)
        {
            if (!mouseDragEndLine)
                base.DragItemBegin(evt);
        }

        #endregion EndLine 鼠标事件监听

        #region Caculate外观

        public override string CaculateName()
        {
            return m_ItemData.GetName();
        }

        public override float CaculatePosiotion(float frameUniWidth)
        {
            return frameIndex * frameUniWidth;
        }

        public override float CaculateWidth(float frameUniWidth)
        {
            return frameUniWidth * m_ItemData.GetFrameDuration(0);
        }

        #endregion Caculate外观

        #region 驱动显示

        private GameObject effectPrefabeObj;

        public void TickView(int currentFrameIndex)
        {
            if (m_ItemData.EffectObject != null)
            {
                int durationFrame = (int)(m_ItemData.GetFrameDuration(0));
                if (currentFrameIndex > m_ItemData.FrameIndex && currentFrameIndex < m_ItemData.FrameIndex + durationFrame)
                {
                    //对比预制体
                    if (effectPrefabeObj != null && effectPrefabeObj.name != m_ItemData.EffectObject.name)
                    {
                        GameObject.DestroyImmediate(effectPrefabeObj);
                    }
                    if (effectPrefabeObj == null)
                    {
                        Transform characterRoot = SkillEditorWindow.Instance.CurrentPreviewGameObject.transform;
                        //原坐标
                        Vector3 oldPos = characterRoot.position;

                        //运行坐标
                        Vector3 rootRotate = this.animationPos;

                        //运行坐标
                        characterRoot.position = rootRotate;
                        Vector3 pos = characterRoot.TransformPoint(m_ItemData.Position);
                        Vector3 rot = characterRoot.eulerAngles + m_ItemData.Rotation;

                        //回到原坐标
                        characterRoot.position = oldPos;

                        //实例化
                        effectPrefabeObj = GameObject.Instantiate(m_ItemData.EffectObject, pos, Quaternion.Euler(rot), EffectTrack.TempEffectPriview.transform);
                        effectPrefabeObj.transform.localScale = m_ItemData.Scale;
                        effectPrefabeObj.name = m_ItemData.EffectObject.name;
                        foreach (var item in effectPrefabeObj.GetComponentsInChildren<ParticleSystem>())
                        {
                            item.randomSeed = (uint)m_ItemData.SeedRandom;//设置随机种子
                        }
                    }

                    ParticleSystem[] particleSystems = effectPrefabeObj.GetComponentsInChildren<ParticleSystem>();
                    for (int i = 0; i < particleSystems.Length; i++)
                    {
                        ParticleSystem.MainModule ps = particleSystems[i].main;
                        ps.loop = false;//禁止循环
                        int simulateFrame = currentFrameIndex - m_ItemData.FrameIndex;
                        particleSystems[i].Simulate((float)simulateFrame / SkillEditorWindow.Instance.SkillConfig.FrameRate);
                    }
                }
                else
                {
                    ClearEffectPrefab();
                }
            }
        }

        public void ApplyTransfromButtonClick()
        {
            if (effectPrefabeObj != null)
            {
                Transform characterRoot = SkillEditorWindow.Instance.CurrentPreviewGameObject.transform;
                //原坐标
                Vector3 oldPos = characterRoot.position;

                //运行坐标
                Vector3 rootRotate = animationPos;

                //运行坐标
                characterRoot.position = rootRotate;
                m_ItemData.Position = characterRoot.InverseTransformPoint(effectPrefabeObj.transform.position);
                m_ItemData.Rotation = effectPrefabeObj.transform.eulerAngles - characterRoot.transform.eulerAngles;
                m_ItemData.Scale = effectPrefabeObj.transform.localScale;

                //回到原坐标
                characterRoot.position = oldPos;
                Debug.Log("成功");
            }
        }

        private void ClearEffectPrefab()
        {
            if (effectPrefabeObj != null)
            {
                GameObject.DestroyImmediate(effectPrefabeObj);
            }
        }

        #endregion 驱动显示

        /// <summary>
        /// 被选中事件
        /// </summary>
        public override void Select()
        {
            SkillEffectEventDataInspectorHelper.Instance.Inspector(ItemData);
            base.Select();
        }
    }
}