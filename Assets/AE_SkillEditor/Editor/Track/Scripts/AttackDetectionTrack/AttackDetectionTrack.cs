using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 多行轨道
    /// </summary>
    public class AttackDetectionTrack : MultiLineTrack<AttackDetectionEvent, AttackDetectionChildTrack>
    {
        //特效物体父对象
        private static GameObject tempAttackDetectioin;

        public static GameObject TempAttackDetectioin
        {
            get
            {
                if (tempAttackDetectioin == null)
                {
                    tempAttackDetectioin = GameObject.Find("====SkillEditorDetections====");
                    if (tempAttackDetectioin == null)
                    {
                        tempAttackDetectioin = new GameObject("====SkillEditorDetections====");
                        tempAttackDetectioin.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    }
                }
                return tempAttackDetectioin;
            }
            private set { tempAttackDetectioin = value; }
        }

        public static string TrackName = "攻击检测轨道";

        public override void TickView(int currentFrameIndex)
        {
            foreach (AttackDetectionChildTrack item in m_ChildTrackList)
            {
                item.TickView(currentFrameIndex);
            }
        }

        public override void ClearScene()
        {
            GameObject.DestroyImmediate(tempAttackDetectioin);
        }
    }

    /// <summary>
    /// 多行轨道子轨道
    /// </summary>
    public class AttackDetectionChildTrack : MultiLineChildTrack<AttackDetectionEvent, AttackDetectionTrackItem>
    {
        public override void Init(MultiLineTrack m_ParentTrack, SkillMultiLineTrackStyle parentStyle, float frameWidth, int index, System.Action<int> DelectChildTrackAction, AttackDetectionEvent m_ItemData)
        {
            base.Init(m_ParentTrack, parentStyle, frameWidth, index, DelectChildTrackAction, m_ItemData);
            //粘贴事件右键菜单
            m_SelfStyle.contentRoot.AddOneRightMenue("添加扇形范围", (res) => { PlaceClip(new RingLikeDetectionParams(), rightButtonClickPos); });
            m_SelfStyle.contentRoot.AddOneRightMenue("添加Cube范围", (res) => { PlaceClip(new CubeDetectionParams(), rightButtonClickPos); });
            m_SelfStyle.contentRoot.AddOneRightMenue("添加球形范围", (res) => { PlaceClip(new SphereDetectionParams(), rightButtonClickPos); });
        }

        /// <summary>
        /// 重写此方法来匹配对应拖拽的类型
        /// </summary>
        /// <returns></returns>
        public override string[] GetObjectType()
        {
            return new string[] { typeof(Object).FullName };
        }

        /// <summary>
        /// 重写此方法来定义Item数据的长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public override int CaculateObjectLength(object clip)
        {
            if (m_ItemData.GetFrameDuration(0) == -1)
                return 30;
            return m_ItemData.GetFrameDuration(0);
        }

        /// <summary>
        /// 驱动显示
        /// </summary>
        /// <param name="currentFrameIndex"></param>
        public override void TickView(int currentFrameIndex)
        {
            if (m_TrackItem != null)
                if (m_TrackItem.ItemData.DetectionParamsBase != null)
                    m_TrackItem.TickView(currentFrameIndex);
        }
    }

    /// <summary>
    /// 多行轨道子轨道的单个Item
    /// </summary>
    public class AttackDetectionTrackItem : MultLineTrackItem<AttackDetectionEvent>
    {
        public override void Init(ChildTrackBase<AttackDetectionEvent> m_ParentTrack, SkillTrackStyleBase m_parentTrackStyle, SkillFrameEventBase m_ItemData, int frameIndex, float frameUniWidth, SimpleItemStyle simpleItemStyle = null)
        {
            base.Init(m_ParentTrack, m_parentTrackStyle, m_ItemData, frameIndex, frameUniWidth);
            //EndLine 为自定义元素绑定事件

            VisualElement animationEndLine = m_SelfStyle.FindElement<VisualElement>("EndLine");

            RegisterMouseEvent<MouseDownEvent>(animationEndLine, EndLineMouseDown);
            RegisterMouseEvent<MouseUpEvent>(animationEndLine, EndLineMouseUp);
            animationEndLine.SetCursor(MouseCursor.SplitResizeLeftRight);
            RegisterMouseEvent<MouseOutEvent>(animationEndLine, EndLineMouseOut);
            animationEndLine.SetCursor(MouseCursor.SplitResizeLeftRight);

            RegsitorRightButtonClickEvent("Main", "对准位置", (res) => { ApplyTransfromButtonClick(); });

            AnimationTrack.PosAction -= PosAction;
            AnimationTrack.PosAction += PosAction;
        }


        Vector3 animationPos = Vector3.zero;

        private void PosAction(Vector3 obj)
        {
            animationPos = obj;
        }


        #region EndLine 鼠标事件监听

        private bool mouseDragEndLine;

        /// <summary>
        /// 重写此方法定义拖拽
        /// </summary>
        /// <param name="evt"></param>
        protected override void DragItem(MouseMoveEvent evt)
        {
            base.DragItem(evt);
            if (mouseDragEndLine)
            {
                float offsetPosx = evt.mousePosition.x - startDragPosx;
                int offsetFrame = Mathf.RoundToInt(offsetPosx / frameUniWidth);

                int targetFrameIndex = startDragFrameIndex + offsetFrame;
                if (targetFrameIndex < 0) return;

                bool checkDrag = false;

                if (offsetFrame > 0) //向右移动
                {
                    checkDrag = m_ParentTrack.CheckFrameIndexOnDrag(targetFrameIndex, frameIndex, false);
                }
                else if (offsetFrame < 0)//向左移动
                {
                    checkDrag = m_ParentTrack.CheckFrameIndexOnDrag(targetFrameIndex, frameIndex, true);
                }

                if (checkDrag)
                {
                    ////更新数据
                    m_ItemData.FrameDuration = targetFrameIndex - frameIndex;
                    SkillEditorWindow.Instance.AutoSaveConfig();
                    ////超过轨道边界自动拓展
                    ////刷新视图
                    ResetRealView(frameUniWidth);
                }
            }
        }

        private void EndLineMouseDown(MouseDownEvent evt)
        {
            mouseDragEndLine = true;
            mouseDrag = false;
            startDragFrameIndex = frameIndex + m_ItemData.FrameDuration;
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

        #region TrackItem 鼠标事件监听

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

        #endregion TrackItem 鼠标事件监听

        #endregion EndLine 鼠标事件监听

        /// <summary>
        /// 重写此属性来定义普通状态颜色
        /// </summary>
        protected override Color m_normalColor => new Color(0.6037736f, 0.2016376f, 0.2016376f, 1f);
        /// <summary>
        /// 重写此属性来定义选中颜色
        /// </summary>
        protected override Color m_selectColor => new Color(0.7169812f, 0.2056247f, 0.2056247f, 1f);

        /// <summary>
        /// 定义Item的 宽度 名字 和位置
        /// </summary>
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

        /// <summary>
        /// 选中
        /// </summary> 
        public override void Select()
        {
            //Debug.Log(ItemData.DetectionParamsBase.GetType().FullName);
            switch (ItemData.DetectionParamsBase.GetType().FullName)
            {
                case "ARPG_AE_JOKER.SkillEditor.SphereDetectionParams":
                    SphereAttackDetectionEventInspectorHelper.Instance.Inspector(ItemData);
                    break;
                case "ARPG_AE_JOKER.SkillEditor.CubeDetectionParams":
                    CubeAttackDetectionEventInspectorHelper.Instance.Inspector(ItemData);
                    break;
                case "ARPG_AE_JOKER.SkillEditor.RingLikeDetectionParams":
                    RingLikeAttackDetectionEventInspectorHelper.Instance.Inspector(ItemData);
                    break;
            }
        }

        private GameObject attackDetectionObj;

        public void TickView(int currentFrameIndex)
        {
            if (m_ItemData.DetectionParamsBase != null)
            {
                int durationFrame = (int)(m_ItemData.GetFrameDuration(0));
                if (currentFrameIndex > m_ItemData.FrameIndex && currentFrameIndex < m_ItemData.FrameIndex + durationFrame)
                {
                    if (attackDetectionObj == null)
                    {
                        if (attackDetectionObj == null)
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

                            attackDetectionObj = new GameObject(ItemData.DetectionParamsBase.GetType().Name);
                            attackDetectionObj.transform.SetParent(AttackDetectionTrack.TempAttackDetectioin.transform);
                            attackDetectionObj.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
                            if (ItemData.DetectionParamsBase is RingLikeDetectionParams)
                            {
                                RingLikeDetection demo = attackDetectionObj.AddComponent<RingLikeDetection>();
                                demo.isDebuge = true;
                                demo.Init(ItemData.DetectionParamsBase as RingLikeDetectionParams);
                            }
                            else if (ItemData.DetectionParamsBase is CubeDetectionParams)
                            {
                                CubeDetection demo = attackDetectionObj.AddComponent<CubeDetection>();
                                demo.isDebuge = true;
                                demo.Init(ItemData.DetectionParamsBase as CubeDetectionParams);
                            }
                            else
                            {
                                SphereDetection demo = attackDetectionObj.AddComponent<SphereDetection>();
                                demo.isDebuge = true;
                                demo.Init(ItemData.DetectionParamsBase as SphereDetectionParams);
                            }
                        }
                    }
                }
                else
                {
                    GameObject.DestroyImmediate(attackDetectionObj);
                }
            }
        }

        public void ApplyTransfromButtonClick()
        {
            if (attackDetectionObj != null)
            {
                Transform characterRoot = SkillEditorWindow.Instance.CurrentPreviewGameObject.transform;
                //原坐标
                Vector3 oldPos = characterRoot.position;

                //运行坐标
                Vector3 rootRotate = animationPos;

                //运行坐标
                characterRoot.position = rootRotate;
                m_ItemData.Position = characterRoot.InverseTransformPoint(attackDetectionObj.transform.position);
                m_ItemData.Rotation = attackDetectionObj.transform.eulerAngles - characterRoot.transform.eulerAngles;

                //回到原坐标
                characterRoot.position = oldPos;
                Debug.Log("成功");
            }
        }
    }
}