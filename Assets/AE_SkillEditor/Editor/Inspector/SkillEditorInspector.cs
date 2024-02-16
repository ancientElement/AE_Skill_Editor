//using System;
//using UnityEditor;
//using UnityEditor.UIElements;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace ARPG_AE_JOKER.SkillEditor
//{
//    [CustomEditor(typeof(SkillEditorWindow))]
//    public class SkillEditorInspector : Editor
//    {
//        private VisualElement root;

//        public static SkillEditorInspector Instance { get; private set; }

//        private static TrackItemBase currentTrackItem;
//        private static TrackBase currenTrack;

//        public SkillFrameEventBase skillFrameEventBase;

//        private int trackItemIndex;//当前TrackItem的帧索引

//        public void SetTrackItemIndex(int index)
//        { trackItemIndex = index; }

//        public override VisualElement CreateInspectorGUI()
//        {
//            Instance = this;
//            root = new VisualElement();
//            return root;
//        }

//        /// <summary>
//        /// 设置当前TrackItem
//        /// </summary>
//        /// <param name="trackItem"></param>
//        /// <param name="skillTrackBase"></param>
//        public static void SetTrackItem(TrackItemBase trackItem, TrackBase skillTrackBase)
//        {
//            if (currentTrackItem != null)
//            {
//                currentTrackItem.UnSelect();
//            }
//            currentTrackItem = trackItem;
//            currentTrackItem.OnSelect();
//            currenTrack = skillTrackBase;
//            if (currentTrackItem != null)
//            {
//                if (Instance != null)
//                    Instance.Show();
//            }
//        }

//        private void OnDestroy()
//        {
//            if (currenTrack != null)
//            {
//                currentTrackItem.UnSelect();
//                currentTrackItem = null;
//                currenTrack = null;
//            }
//        }

//        /// <summary>
//        /// 显示Inspector
//        /// </summary>
//        private void Show()
//        {
//            Clear();
//            if (currentTrackItem != null)
//            {
//                //TOOD:绘制TrackItem数据
//                Type type = currentTrackItem.GetType();
//                if (type == typeof(AnimationTrackItem))
//                {
//                    DrawAnimationTrackItemInapactor(currentTrackItem as AnimationTrackItem);
//                }
//                if (type == typeof(AudioTrackItem))
//                {
//                    DrawAudiioTrackItemInapactor(currentTrackItem as AudioTrackItem);
//                }
//                if (type == typeof(EffectTrackItem))
//                {
//                    DrawEffectTrackItemInapactor(currentTrackItem as EffectTrackItem);
//                }
//                else
//                {
//                }
//            }
//        }

//        #region AUTO

//        //obj
//        public ObjectField CreateObjectFiled<T>(string name, T value, Action<T> callback) where T : UnityEngine.Object
//        {
//            ObjectField controller = new ObjectField(name);
//            controller.objectType = typeof(T);
//            controller.value = value;
//            controller.RegisterValueChangedCallback((evt) => { ObjectFiledValueChanged<T>(evt, controller, callback); });
//            return controller;
//        }

//        private void ObjectFiledValueChanged<T>(ChangeEvent<UnityEngine.Object> evt, ObjectField controller, Action<T> callback) where T : UnityEngine.Object
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                T temp = evt.newValue as T;
//                controller.value = temp;
//                callback(temp);
//            }
//        }

//        //v3flied
//        public Vector3Field CreateV3Filed(string name, Vector3 value, Action<Vector3> callback)
//        {
//            Vector3Field controller = new Vector3Field(name);
//            controller.value = value;
//            controller.RegisterValueChangedCallback((evt) => { V3FiledValueChanged(evt, controller, callback); });
//            return controller;
//        }

//        private void V3FiledValueChanged(ChangeEvent<Vector3> evt, Vector3Field controller, Action<Vector3> callback)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                controller.value = evt.newValue;
//                callback(evt.newValue);
//            }
//        }

//        //bool
//        public Toggle CreateBool(string name, bool value, Action<bool> callback)
//        {
//            Toggle controller = new Toggle(name);
//            controller.value = value;
//            controller.RegisterValueChangedCallback((evt) => { BoolValueChanged(evt, controller, callback); });
//            return controller;
//        }

//        private void BoolValueChanged(ChangeEvent<bool> evt, Toggle controller, Action<bool> callback)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                controller.value = evt.newValue;
//                callback(evt.newValue);
//            }
//        }

//        //float
//        public FloatField CreateFloat(string name, float value, Action<float> callback)
//        {
//            FloatField controller = new FloatField(name);
//            controller.value = value;
//            controller.RegisterValueChangedCallback((evt) => { FloatValueChanged(evt, controller, callback); });
//            return controller;
//        }

//        private void FloatValueChanged(ChangeEvent<float> evt, FloatField controller, Action<float> callback)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                controller.value = evt.newValue;
//                callback(evt.newValue);
//            }
//        }

//        //int
//        public IntegerField Createint(string name, int value, Action<int> callback)
//        {
//            IntegerField controller = new IntegerField(name);
//            controller.value = value;
//            controller.RegisterValueChangedCallback((evt) => { IntValueChanged(evt, controller, callback); });
//            return controller;
//        }

//        private void IntValueChanged(ChangeEvent<int> evt, IntegerField controller, Action<int> callback)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                controller.value = evt.newValue;
//                callback(evt.newValue);
//            }
//        }

//        //private void ValueChanged<Tvalue,Tcontroller>(ChangeEvent<Tvalue> evt, Tcontroller controller, Action<Tvalue> callback) where Tcontroller : BaseField<Tvalue>  where Tvalue : struct
//        //{
//        //    if (evt.previousValue != evt.newValue)
//        //    {
//        //        controller.value = evt.newValue;
//        //        callback(evt.newValue);
//        //    }
//        //}

//        #endregion AUTO

//        #region 特效轨道Inspector

//        private void DrawEffectTrackItemInapactor(EffectTrackItem effectTrackItem)
//        {
//            IntegerField durationField = default;

//            //特效资源
//            ObjectField effectField = CreateObjectFiled<GameObject>("特效资源", effectTrackItem.ItemData.EffectObject, (obj) =>
//            {
//                ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
//                float maxvalue = 0;
//                int max = 0;
//                for (int i = 0; i < particleSystems.Length; i++)
//                {
//                    if (particleSystems[i].main.duration > maxvalue)
//                    {
//                        max = i;
//                        maxvalue = particleSystems[i].main.duration;
//                    }
//                }

//                //保存到配置中
//                (currentTrackItem as EffectTrackItem).ItemData.FrameDuration = (int)(particleSystems[max].main.duration * SkillEditorWindow.Instance.SkillConfig.FrameRate);
//                (currentTrackItem as EffectTrackItem).ItemData.EffectObject = obj;

//                durationField.value = (currentTrackItem as EffectTrackItem).ItemData.FrameDuration;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//            });
//            root.Add(effectField);

//            //位置
//            Vector3Field posField = CreateV3Filed("位置", effectTrackItem.ItemData.Position, (res) =>
//            {
//                (currentTrackItem as EffectTrackItem).ItemData.Position = res;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//                //TODO:改变实际位置
//            });
//            root.Add(posField);

//            //旋转
//            Vector3Field rotField = CreateV3Filed("旋转", effectTrackItem.ItemData.Rotation, (res) =>
//            {
//                (currentTrackItem as EffectTrackItem).ItemData.Rotation = res;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//                //TODO:改变实际位置
//            });
//            root.Add(rotField);

//            //位置
//            Vector3Field scaleField = CreateV3Filed("缩放", effectTrackItem.ItemData.Scale, (res) =>
//            {
//                (currentTrackItem as EffectTrackItem).ItemData.Scale = res;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//                //TODO:改变实际位置
//            });
//            root.Add(scaleField);

//            //自动销毁
//            Toggle autoDestroy = CreateBool("自动销毁", effectTrackItem.ItemData.AutoDestroy, (res) =>
//            {
//                (currentTrackItem as EffectTrackItem).ItemData.AutoDestroy = res;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//            });
//            root.Add(autoDestroy);

//            //持续时间
//            durationField = Createint("持续时间", effectTrackItem.ItemData.FrameDuration, (res) =>
//            {
//                (currentTrackItem as EffectTrackItem).ItemData.FrameDuration = res;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//            });
//            root.Add(durationField);

//            //引用transfrom属性
//            //删除按钮
//            Button applyTransfromButton = new Button(ApplyTransfromButtonClick);
//            applyTransfromButton.text = "引用transfrom属性";
//            applyTransfromButton.style.backgroundColor = Color.red;
//            root.Add(applyTransfromButton);
//        }

//        private void ApplyTransfromButtonClick()
//        {
//            (currentTrackItem as EffectTrackItem).ApplyTransfromButtonClick();
//            Show();
//        }

//        #endregion 特效轨道Inspector

//        #region 音效轨道Inspector

//        //Toggle applyRootMotionToggle;
//        private FloatField volumnField;

//        private void DrawAudiioTrackItemInapactor(AudioTrackItem animationTrackItem)
//        {
//            //音效资源
//            ObjectField audioClipField = CreateObjectFiled<AudioClip>("音效资源", animationTrackItem.ItemData.AudioClip, (res) =>
//            {
//                //保存到配置中
//                (currentTrackItem as AudioTrackItem).ItemData.AudioClip = res;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//            });
//            root.Add(audioClipField);
//            //音量
//            volumnField = CreateFloat("音量", animationTrackItem.ItemData.volumn, (res) =>
//            {
//                //保存到配置中
//                (currentTrackItem as AudioTrackItem).ItemData.volumn = res;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//            });
//            root.Add(volumnField);
//        }

//        #endregion 音效轨道Inspector

//        #region 动画轨道Inspector

//        private Toggle applyRootMotionToggle;
//        private ObjectField animationClipField;
//        private IntegerField duratuionField;
//        private FloatField translationField;
//        private Label clipFrameCountLable;
//        private Label isLooplable;
//        private Button deleteButton;

//        /// <summary>
//        /// 绘制动画Inspectors
//        /// </summary>
//        private void DrawAnimationTrackItemInapactor(AnimationTrackItem animationTrackItem)
//        {
//            trackItemIndex = animationTrackItem.FrameIndex;

//            //动画资源
//            animationClipField = new ObjectField("动画资源");
//            animationClipField.objectType = typeof(AnimationClip);
//            animationClipField.value = animationTrackItem.ItemData.AnimationClip;
//            animationClipField.RegisterValueChangedCallback(AnimationClipValueChanged);
//            root.Add(animationClipField);

//            //根运动
//            applyRootMotionToggle = new Toggle("应用根运动");
//            applyRootMotionToggle.value = animationTrackItem.ItemData.ApplyRootMotion;
//            applyRootMotionToggle.RegisterValueChangedCallback(ApplyRootMotionToggleValueChanged);
//            root.Add(applyRootMotionToggle);

//            //定义动画长度
//            duratuionField = new IntegerField("定义动画长度");
//            duratuionField.value = animationTrackItem.ItemData.DurationFrame;
//            duratuionField.RegisterValueChangedCallback(DuratuionFieldValueChanged);
//            root.Add(duratuionField);

//            //过渡事件
//            translationField = new FloatField("过渡时间");
//            translationField.value = animationTrackItem.ItemData.TransitionTime;
//            translationField.RegisterValueChangedCallback(TranslationFieldldValueChanged);
//            root.Add(translationField);

//            //实际动画长度
//            int clipFrameCount = (int)(animationTrackItem.ItemData.AnimationClip.length * animationTrackItem.ItemData.AnimationClip.frameRate);
//            clipFrameCountLable = new Label("实际动画长度: " + clipFrameCount.ToString());
//            root.Add(clipFrameCountLable);

//            //循环播放
//            isLooplable = new Label("循环动画: " + animationTrackItem.ItemData.AnimationClip.isLooping);
//            root.Add(isLooplable);

//            //删除按钮
//            deleteButton = new Button(DeleteButtonClicked);
//            deleteButton.text = "删除";
//            deleteButton.style.backgroundColor = Color.red;
//            root.Add(deleteButton);
//        }

//        /// <summary>
//        /// 跟运动改变
//        /// </summary>
//        /// <param name="evt"></param>
//        private void ApplyRootMotionToggleValueChanged(ChangeEvent<bool> evt)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                (currentTrackItem as AnimationTrackItem).ItemData.ApplyRootMotion = evt.newValue;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//            }
//        }

//        /// <summary>
//        /// 动画资源修改
//        /// </summary>
//        /// <param name="evt"></param>
//        private void AnimationClipValueChanged(ChangeEvent<UnityEngine.Object> evt)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                AnimationClip clip = evt.newValue as AnimationClip;
//                animationClipField.value = clip;
//                clipFrameCountLable.text = "实际动画长度: " + (int)(clip.length * clip.frameRate);
//                isLooplable.text = "循环动画: " + clip.isLooping;
//                //保存到配置中
//                (currentTrackItem as AnimationTrackItem).ItemData.AnimationClip = clip;
//                SkillEditorWindow.Instance.AutoSaveConfig();
//                currentTrackItem.ResetView();
//            }
//        }

//        /// <summary>
//        /// 定义动画长度
//        /// </summary>
//        /// <param name="evt"></param>
//        private void DuratuionFieldValueChanged(ChangeEvent<int> evt)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                int value = evt.newValue;
//                if ((currenTrack as AnimationTrack).CheckFrameIndexOnDrag(trackItemIndex + value, trackItemIndex, false))
//                {
//                    //保存到配置中
//                    (SkillEditorWindow.Instance.SkillConfig.trackDataDic["ARPG_AE_JOKER.SkillEditor.AnimationTrack"] as SkillSingLineTrackDataBase<SkillAnimationEvent>).FrameData[trackItemIndex].DurationFrame = value;
//                    (currentTrackItem as AnimationTrackItem).CheckFrameCount();
//                    SkillEditorWindow.Instance.AutoSaveConfig();
//                    currentTrackItem.ResetView();
//                }
//                else
//                {
//                    duratuionField.value = evt.previousValue;
//                }
//            }
//        }

//        /// <summary>
//        /// 过渡时间
//        /// </summary>
//        /// <param name="evt"></param>
//        private void TranslationFieldldValueChanged(ChangeEvent<float> evt)
//        {
//            if (evt.previousValue != evt.newValue)
//            {
//                (currentTrackItem as AnimationTrackItem).ItemData.TransitionTime = evt.newValue;
//            }
//        }

//        /// <summary>
//        /// 删除按钮
//        /// </summary>
//        private void DeleteButtonClicked()
//        {
//            currenTrack.DelectTrackItem(trackItemIndex);
//            Selection.activeObject = null;
//        }

//        #endregion 动画轨道Inspector

//        /// <summary>
//        /// 清理Inspector
//        /// </summary>
//        private void Clear()
//        {
//            if (root != null)
//            {
//                for (int i = root.childCount - 1; i >= 0; i--)
//                {
//                    root.RemoveAt(i);
//                }
//            }
//        }
//    }
//}