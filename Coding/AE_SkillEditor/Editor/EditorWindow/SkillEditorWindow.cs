using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using Unity.EditorCoroutines.Editor;

namespace ARPG_AE_JOKER.SkillEditor
{
    public  class SkillEditorWindow : EditorWindow
    {
        public static SkillEditorWindow Instance { get; private set; }

        private VisualElement root;

        private const string stylePath = "Assets/AE_SkillEditor/Editor/Track/Assets/Uss/Extend.uss";
        private const string AssetPath = "Assets/AE_SkillEditor/Editor/EditorWindow/SkillEiditorWindow.uxml";

        [MenuItem("SkillEditor/SkillEiditorWindow")]
        public static void ShowExample()
        {
            SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
            wnd.titleContent = new GUIContent("技能编辑器 ");
        }

        private void OnDestroy()
        {
            if (skillConfig != null)
                AutoSaveConfig();
        }

        public void CreateGUI()
        {
            SkillConfig.SetSkillValidateAction(ResetView);

            Instance = this;

            root = rootVisualElement;
            StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylePath);
            root.styleSheets.Add(uss);

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath);

            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);
            InitTopMenu();
            InitTimeShaft();
            InitContent();
            InitConsole();

            //配置
            if (skillConfig != null)
            {
                CurrentFrameCount = skillConfig.FrameCount;
            }
            else { CurrentFrameCount = 100; }

            CurrentSelectFrameIndex = 0;

            //刷新面板
            CurrentFrameTextField.value = CurrentSelectFrameIndex;
            FrameCountTextFiled.value = CurrentFrameCount;
        }

        public void ResetView()
        {
            SkillConfig tempSkillConfig = skillConfig;
            SkillConfigObjectField.value = null;
            SkillConfigObjectField.value = tempSkillConfig;
        }

        #region Config

        private SkillConfig skillConfig;

        public SkillConfig SkillConfig
        {
            get
            {
                return skillConfig;
            }
        }

        private SkillEditorConfig skillEditorConfig = new SkillEditorConfig();

        //当前选中帧
        private int currentSelectFrameIndex = -1;

        //当前选中帧
        public int CurrentSelectFrameIndex
        {
            get => currentSelectFrameIndex;
            private set
            {
                if (value != currentSelectFrameIndex)
                {
                    currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentFrameCount);
                    //面板同步
                    CurrentFrameTextField.value = currentSelectFrameIndex;
                    UpdateTimeShaftView();
                    TickSkill();
                }
            }
        }

        //总帧数
        private int currentFrameCount;

        //总帧数
        public int CurrentFrameCount
        {
            get => currentFrameCount;
            set
            {
                if (value != currentFrameCount)
                {
                    currentFrameCount = value;
                    //同步面板
                    FrameCountTextFiled.value = value;
                    //同步给skillConfig
                    if (skillConfig != null) { skillConfig.FrameCount = value; }
                    //时间轴发送变化ContentView缩放
                    UpdateContentSise();
                }
            }
        }

        #endregion Config

        #region 顶部

        private const string skillEditorScenePath = "Assets/AE_SkillEditor/Scene/SkillEditorScene.unity";
        private const string priviewGameObjectPath = "PreviewCharacterRoot";
        private string oldScenePath;

        private ToolbarButton LoadEditorSceneButton;

        private ToolbarButton SkillBasicButton;

        private ToolbarButton CreateSkillTrackButton;

        private DropdownField SelectSceneDrapDown;

        private ObjectField PreviewCharacterPrefabObjectField;
        private ObjectField SkillConfigObjectField;
        private Label SkillConfigNameLabel;
        private Label PreviewCharacterGONameLabel;
        private ToolbarButton SaveConfigButton;
        private Toggle AutoSaveToggle;

        private GameObject currentPreviewGameObject;
        private GameObject currentPreviewGameObjectPrefab;
        public GameObject CurrentPreviewGameObject { get => currentPreviewGameObject; }

        public bool isOnEditorScene
        { get { return EditorSceneManager.GetActiveScene().path == skillEditorScenePath; } }

        private void InitTopMenu()
        {
            LoadEditorSceneButton = root.Q<ToolbarButton>(nameof(LoadEditorSceneButton));
            LoadEditorSceneButton.clicked += LoadEditorSceneButtonClick;

            SelectSceneDrapDown = root.Q<DropdownField>(nameof(SelectSceneDrapDown));
            SelectSceneDrapDown.choices.Clear();

            PreviewCharacterGONameLabel = root.Q<Label>(nameof(PreviewCharacterGONameLabel));
            SkillConfigNameLabel = root.Q<Label>(nameof(SkillConfigNameLabel));

            SelectSceneDrapDown.RegisterValueChangedCallback(LoadScene);

            SkillBasicButton = root.Q<ToolbarButton>(nameof(SkillBasicButton));
            SkillBasicButton.clicked += SkillBasicButtonClick;

            PreviewCharacterPrefabObjectField = root.Q<ObjectField>(nameof(PreviewCharacterPrefabObjectField));
            PreviewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldRegister);

            SkillConfigObjectField = root.Q<ObjectField>(nameof(SkillConfigObjectField));
            SkillConfigObjectField.objectType = typeof(ARPG_AE_JOKER.SkillEditor.SkillConfig);
            SkillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldRegister);

            SaveConfigButton = root.Q<ToolbarButton>(nameof(SaveConfigButton));
            SaveConfigButton.clicked += SaveConfig;

            AutoSaveToggle = root.Q<Toggle>(nameof(AutoSaveToggle));
            AutoSaveToggle.RegisterValueChangedCallback(AutoSaveValueChanged);

            CreateSkillTrackButton = root.Q<ToolbarButton>(nameof(CreateSkillTrackButton));
            CreateSkillTrackButton.clicked += OpenCreateTrackWindow;

            if (skillConfig != null)
            {
                SkillConfigNameLabel.text = skillConfig.name;
            }
            if (currentPreviewGameObject != null)
            {
                PreviewCharacterGONameLabel.text = currentPreviewGameObjectPrefab.name;
            }

            //选择场景初始化
            foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
            {
                string scenesName = S.path;
                string[] Name = scenesName.Split('/');
                foreach (var item in Name)
                {
                    if (item.Contains(".unity"))
                    {
                        SelectSceneDrapDown.choices.Add(item);
                    }
                }
            }

#if UNITY_EDITOR
            ReLoadPrefab();
#endif
        }

        private void OpenCreateTrackWindow()
        {
            CreateSkillTrackWindow.ShowExample();
            CreateSkillTrackWindow.Instance.Init(skillConfig);
        }

        /// <summary>
        /// 自动保存
        /// </summary>
        /// <param name="evt"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AutoSaveValueChanged(ChangeEvent<bool> evt)
        {
            skillEditorConfig.isAutoSaveConfig = evt.newValue;
        }

        /// <summary>
        /// 技能配置文件选择事件
        /// </summary>
        /// <param name="evt"></param>
        private void SkillConfigObjectFieldRegister(ChangeEvent<UnityEngine.Object> evt)
        {
            if (evt.newValue != evt.previousValue)
            {
                skillConfig = evt.newValue as SkillConfig;
                CurrentSelectFrameIndex = 0;
                UpdateTimeShaftView();
                if (SkillConfig != null)
                {
                    CurrentFrameCount = skillConfig.FrameCount;
                    FPSDropDownField.value = skillConfig.FrameRate.ToString();
                    SkillConfigNameLabel.text = skillConfig.skillName;
                }
                else
                {
                    CurrentFrameCount = 100;
                }
                //初始化轨道
                InitTrack();
            }
        }

        /// <summary>
        /// 角色预制体选择事件
        /// </summary>
        /// <param name="evt"></param>
        private void PreviewCharacterPrefabObjectFieldRegister(ChangeEvent<UnityEngine.Object> evt)
        {
            //避免其他场景
            if (evt.newValue != evt.previousValue)
            {
                string current = EditorSceneManager.GetActiveScene().path;
                if (current == skillEditorScenePath)
                {
                    //有预览对象 销毁
                    if (currentPreviewGameObject != null)
                    {
                        DestroyImmediate(currentPreviewGameObject);
                    }

                    //没有 销毁跟对象的子对象
                    Transform priviewRoot = GameObject.Find(priviewGameObjectPath).transform;
                    if (priviewRoot != null && priviewRoot.childCount > 0)
                    {
                        DestroyImmediate(priviewRoot.GetChild(0).gameObject);
                    }

                    //生成新物体
                    if (evt.newValue != null)
                    {
                        currentPreviewGameObject = Instantiate(evt.newValue as GameObject, priviewRoot);
                        currentPreviewGameObject.transform.localRotation = Quaternion.identity;
                        PreviewCharacterGONameLabel.text = currentPreviewGameObject.name;
                    }
                    currentPreviewGameObjectPrefab = evt.newValue as GameObject;
                }
            }
        }

        /// <summary>
        /// 加载编辑器场景
        /// </summary>
        private void LoadEditorSceneButtonClick()
        {
            string current = EditorSceneManager.GetActiveScene().path;
            //非编辑器场景,加载
            if (current != skillEditorScenePath)
            {
                oldScenePath = current;
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                EditorSceneManager.OpenScene(skillEditorScenePath);
                ReLoadPrefab();
                SelectSceneDrapDown.value = "";
            }
        }

        /// <summary>
        /// 回归旧场景
        /// </summary>
        private void LoadOldSceneButtonClick()
        {
            if (!string.IsNullOrEmpty(oldScenePath))
            {
                string current = EditorSceneManager.GetActiveScene().path;
                if (current != oldScenePath)
                {
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                    EditorSceneManager.OpenScene(oldScenePath);
                }
            }
            else
            {
                Debug.LogWarning("场景不存在!");
            }
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="evt"></param>
        private void LoadScene(ChangeEvent<string> evt)
        {
            if (evt.newValue != evt.previousValue)
            {
                //非同一个场景
                if (evt.newValue != evt.previousValue && evt.newValue != "")
                {
                    foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
                    {
                        string scenesName = S.path;
                        string[] Name = scenesName.Split('/');
                        foreach (var item in Name)
                        {
                            if (item.Contains(evt.newValue))
                            {
                                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                                EditorSceneManager.OpenScene(scenesName);
                                oldScenePath = evt.newValue;
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查看技能信息
        /// </summary>
        private void SkillBasicButtonClick()
        {
            if (skillConfig != null)
            {
                //unity聚焦
                Selection.activeObject = skillConfig;
            }
        }

        /// <summary>
        /// 恢复之前的prefab
        /// </summary>
        private void ReLoadPrefab()
        {
            if (EditorSceneManager.GetActiveScene().path == skillEditorScenePath)
                if (currentPreviewGameObjectPrefab != null)
                {
                    PreviewCharacterPrefabObjectField.value = currentPreviewGameObjectPrefab;
                    //有预览对象 销毁
                    if (currentPreviewGameObject != null)
                    {
                        DestroyImmediate(currentPreviewGameObject);
                    }

                    //没有 销毁跟对象的子对象
                    Transform priviewRoot = GameObject.Find(priviewGameObjectPath).transform;
                    if (priviewRoot != null && priviewRoot.childCount > 0)
                    {
                        DestroyImmediate(priviewRoot.GetChild(0).gameObject);
                    }
                    //生成新物体
                    currentPreviewGameObject = Instantiate(currentPreviewGameObjectPrefab, priviewRoot);
                    currentPreviewGameObject.transform.localRotation = Quaternion.identity;
                }
        }

        #endregion 顶部菜单

        #region 时间轴

        private IMGUIContainer timeShaft;
        private IMGUIContainer selectLine;
        private VisualElement contentContainer;
        private VisualElement contentViewPort;

        private bool timeShaftIsMouseEnter;//鼠标进入时间轴

        private float contentOffsetPos { get => Mathf.Abs(contentContainer.transform.position.x); }//滚动条偏移坐标
        private float currentSelectFramePos { get => CurrentSelectFrameIndex * skillEditorConfig.frameUniWidth; }//选中帧的x坐标

        private void InitTimeShaft()
        {
            ScrollView mainContentView = root.Q<ScrollView>("MainContentView");
            contentContainer = mainContentView.Q<VisualElement>("unity-content-container");
            contentViewPort = mainContentView.Q<VisualElement>("unity-content-viewport");

            timeShaft = root.Q<IMGUIContainer>("TimeShaft");
            timeShaft.onGUIHandler = DrawTimeShaft;//绘制时间轴
            timeShaft.RegisterCallback<WheelEvent>(TimeShaftWheel);
            timeShaft.RegisterCallback<MouseDownEvent>(TimeShaftMouseDownEvent);
            timeShaft.RegisterCallback<MouseMoveEvent>(TimeShaftMouseMoveEvent);
            timeShaft.RegisterCallback<MouseUpEvent>(TimeShaftMouseUpEvent);
            timeShaft.RegisterCallback<MouseOutEvent>(TimeShaftMouseOutEvent);

            contentContainer.RegisterCallback<MouseMoveEvent>(TimeShaftMouseMoveEvent);
            contentContainer.RegisterCallback<MouseUpEvent>(TimeShaftMouseUpEvent);
            contentContainer.RegisterCallback<MouseOutEvent>(TimeShaftMouseOutEvent);

            selectLine = root.Q<IMGUIContainer>("SelectLine");
            selectLine.onGUIHandler = DrawSelectLLine;
        }

        /// <summary>
        /// 鼠标位置得到选中帧索引
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int GetFrameIndexFromMousePosition(float x)
        {
            return GetFrameIndexPos(contentOffsetPos + x);
        }

        /// <summary>
        /// 鼠标位置得到选中帧索引
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int GetFrameIndexPos(float x)
        {
            return Mathf.RoundToInt(x / skillEditorConfig.frameUniWidth);
        }

        /// <summary>
        /// 更新时间轴显示
        /// </summary>
        private void UpdateTimeShaftView()
        {
            timeShaft.MarkDirtyLayout();
            selectLine.MarkDirtyLayout();
        }

        /// <summary>
        /// 绘制选中线
        /// </summary>
        private void DrawSelectLLine()
        {
            //判断选中帧是否在范围类
            if (currentSelectFramePos >= contentOffsetPos)
            {
                Handles.BeginGUI();
                Handles.color = Color.red;
                float x = currentSelectFramePos - contentOffsetPos;
                Handles.DrawLine(new Vector3(x, 10), new Vector3(x, timeShaft.contentRect.height + contentViewPort.contentRect.height));
                GUI.color = Color.yellow;
                GUI.Label(new Rect(x - x.ToString().Length * 2.6f, 25, 30, 20), CurrentSelectFrameIndex.ToString());
                Handles.EndGUI();
            }
        }

        /// <summary>
        /// 滚轮事件
        /// </summary>
        /// <param name="evt"></param>
        private void TimeShaftWheel(WheelEvent evt)
        {
            int delta = (int)evt.delta.y;
            skillEditorConfig.frameUniWidth = Mathf.Clamp(skillEditorConfig.frameUniWidth - delta / 3, 1, SkillEditorConfig.standFrameUniWidth * SkillEditorConfig.maxFrameScaleLV);
            UpdateTimeShaftView();
            UpdateContentSise();
            if (SkillConfig != null)
            {
                ResetView(skillEditorConfig.frameUniWidth);
            }
        }

        /// <summary>
        /// 绘制时间轴
        /// </summary>
        private void DrawTimeShaft()
        {
            //开始绘制
            Handles.BeginGUI();
            Handles.color = Color.white;

            Rect rect = timeShaft.contentRect;//时间轴rect

            //起始帧数
            //98 / 10 = 9.8
            int index = Mathf.CeilToInt(contentOffsetPos / skillEditorConfig.frameUniWidth);

            //起始偏移
            float startOffset = 0f;
            if (index > 0)
            {
                // 98 % 10 = 8
                //10 - 8 = 2
                //向上取整的余数
                startOffset = skillEditorConfig.frameUniWidth - (contentOffsetPos % skillEditorConfig.frameUniWidth);
            }

            //Debug.Log(index + "__" + startOffset);

            //int tickStep = 5;//步长
            int tickStep = SkillEditorConfig.maxFrameScaleLV + 1 - (skillEditorConfig.frameUniWidth / SkillEditorConfig.standFrameUniWidth);
            tickStep = tickStep / 2;
            tickStep = tickStep == 0 ? 1 : tickStep;

            if ((skillEditorConfig.frameUniWidth / SkillEditorConfig.standFrameUniWidth) < 1)
            {
                tickStep = (int)(2.7f + 2.3f * (SkillEditorConfig.standFrameUniWidth / skillEditorConfig.frameUniWidth));
            }

            for (float i = startOffset; i < rect.width; i += skillEditorConfig.frameUniWidth)//i是x轴坐标 单位像素
            {
                //绘制长线和文本
                if (index % tickStep == 0)
                {
                    //Handles.DrawLine 坐标系 锚点左上角 从 p1 到 p2
                    Handles.DrawLine(new Vector3(i, rect.height * (1 - 0.3f)), new Vector3(i, rect.height));
                    string indexLabel = index.ToString();
                    GUI.Label(new Rect(i - indexLabel.Length * 4.5f, rect.y, 30, 20), indexLabel);
                }
                else
                {
                    Handles.DrawLine(new Vector3(i, rect.height * (1 - 0.1f)), new Vector3(i, rect.height));
                }
                index += 1;
            }

            //结束绘制
            Handles.EndGUI();
        }

        #region 时间轴鼠标事件

        private void TimeShaftMouseDownEvent(MouseDownEvent evt)
        {
            timeShaftIsMouseEnter = true;
            IsPlaying = false;
            //获取选中帧索引
            if (GetFrameIndexFromMousePosition(evt.localMousePosition.x) != CurrentSelectFrameIndex)
            {
                CurrentSelectFrameIndex = GetFrameIndexFromMousePosition(evt.localMousePosition.x);
            }
        }

        private void TimeShaftMouseMoveEvent(MouseMoveEvent evt)
        {
            if (timeShaftIsMouseEnter)
            {
                //获取选中帧索引
                if (GetFrameIndexFromMousePosition(evt.localMousePosition.x) != CurrentSelectFrameIndex)
                {
                    CurrentSelectFrameIndex = GetFrameIndexFromMousePosition(evt.localMousePosition.x);
                }
            }
        }

        private void TimeShaftMouseUpEvent(MouseUpEvent evt)
        {
            timeShaftIsMouseEnter = false;
        }

        private void TimeShaftMouseOutEvent(MouseOutEvent evt)
        {
            timeShaftIsMouseEnter = false;
        }

        #endregion 时间轴鼠标事件

        #endregion 时间轴

        #region 控制台

        private ToolbarButton PreviousFrameButton;
        private ToolbarButton PlayButton;
        private ToolbarButton NextFrameButton;
        private ToolbarButton ToTheEndButton;
        private ToolbarButton ToTheHeadButton;
        private IntegerField CurrentFrameTextField;
        private IntegerField FrameCountTextFiled;
        private DropdownField FPSDropDownField;
        private ToolbarButton ClearSceneTopBarButton;

        private FloatField ConsoleAnimationSpeed;
        public float animationSpeed = 1f;

        private void InitConsole()
        {
            PreviousFrameButton = root.Q<ToolbarButton>("PreviousFrameButton");
            PreviousFrameButton.clicked += PreviousFrameButtonClick;

            PlayButton = root.Q<ToolbarButton>("PlayButton");
            PlayButton.clicked += PlayButtonClick;

            NextFrameButton = root.Q<ToolbarButton>("NextFrameButton");
            NextFrameButton.clicked += NextFrameButtonClick;

            CurrentFrameTextField = root.Q<IntegerField>("CurrentFrameTextField");
            CurrentFrameTextField.RegisterValueChangedCallback(CurrentFrameTextFieldValueChange);

            FrameCountTextFiled = root.Q<IntegerField>("FrameCountTextFiled");
            FrameCountTextFiled.RegisterValueChangedCallback(FrameCountTextFiledValueChange);

            ToTheEndButton = root.Q<ToolbarButton>("ToTheEndButton");
            ToTheEndButton.clicked += ToTheEndButtonClick;

            ToTheHeadButton = root.Q<ToolbarButton>("ToTheHeadButton");
            ToTheHeadButton.clicked += ToTheHeadButtonClick;

            ClearSceneTopBarButton = root.Q<ToolbarButton>("ClearSceneTopBarButton");
            ClearSceneTopBarButton.clicked += ClearSceneTopBarButtonClick;

            FPSDropDownField = root.Q<DropdownField>("FPSDropDownField");
            FPSDropDownField.RegisterValueChangedCallback(FPSDropDownFieldValueChange);

            ConsoleAnimationSpeed = root.Q<FloatField>("ConsoleAnimationSpeed");
            ConsoleAnimationSpeed.RegisterValueChangedCallback(ConsoleAnimationSpeedFieldValueChange);
            ConsoleAnimationSpeed.value = animationSpeed;

            if (skillConfig != null)
            {
                SkillConfigObjectField.value = skillConfig;
            }
        }


        //清理场景
        private void ClearSceneTopBarButtonClick()
        {
            foreach (TrackBase item in trackList)
            {
                item.ClearScene();
            }
        }

        private void ConsoleAnimationSpeedFieldValueChange(ChangeEvent<float> evt)
        {
            if (evt.newValue != evt.previousValue)
            {
                animationSpeed = evt.newValue;
            }
        }

        /// <summary>
        /// 改变帧率
        /// </summary>
        /// <param name="evt"></param>
        private void FPSDropDownFieldValueChange(ChangeEvent<string> evt)
        {
            if (evt.previousValue != evt.newValue)
            {
                if (skillConfig != null)
                {
                    skillConfig.FrameRate = int.Parse(evt.newValue);
                }
            }
        }

        /// <summary>
        /// 回到开头
        /// </summary>
        private void ToTheHeadButtonClick()
        {
            CurrentSelectFrameIndex = 0;
        }

        /// <summary>
        /// 会到结尾
        /// </summary>
        private void ToTheEndButtonClick()
        {
            CurrentSelectFrameIndex = CurrentFrameCount;
        }

        /// <summary>
        /// 当前帧文本框变化事件
        /// </summary>
        /// <param name="evt"></param>
        private void CurrentFrameTextFieldValueChange(ChangeEvent<int> evt)
        {
            if (evt.newValue != evt.previousValue)
            {
                if (evt.newValue != CurrentSelectFrameIndex)
                {
                    int num;
                    try
                    {
                        num = evt.newValue;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    CurrentSelectFrameIndex = num;
                }
            }
        }

        /// <summary>
        /// 帧总数文本框变化事件
        /// </summary>
        /// <param name="evt"></param>
        private void FrameCountTextFiledValueChange(ChangeEvent<int> evt)
        {
            if (evt.newValue != evt.previousValue)
            {
                if (evt.newValue != CurrentFrameCount)
                {
                    int num;
                    try
                    {
                        num = evt.newValue;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    CurrentFrameCount = num;
                }
            }
        }

        /// <summary>
        /// 下一帧按钮点击
        /// </summary>
        private void NextFrameButtonClick()
        {
            CurrentSelectFrameIndex += 1;
            IsPlaying = false;
        }

        /// <summary>
        /// 播放按钮
        /// </summary>
        private void PlayButtonClick()
        {
            IsPlaying = !IsPlaying;
        }

        /// <summary>
        /// 上一帧按钮点击
        /// </summary>
        private void PreviousFrameButtonClick()
        {
            CurrentSelectFrameIndex -= 1;
            IsPlaying = false;
        }

        #endregion 控制台

        #region 轨道

        private VisualElement ContentListView;//右侧内容
        private VisualElement TrackMenuParent;//轨道菜单
        private ScrollView MainContentView;   //右侧滑动

        private List<TrackBase> trackList = new List<TrackBase>();

        /// <summary>
        /// 同步左右上下滑动
        /// </summary>
        /// <param name="value"></param>
        private void MainContentViewVerticalValueChanged(float value)
        {
            Vector3 pos = TrackMenuParent.transform.position;
            pos.y = contentContainer.transform.position.y;
            TrackMenuParent.transform.position = pos;
        }

        /// <summary>
        /// 更新容器体积
        /// </summary>
        private void UpdateContentSise()
        {
            if (ContentListView != null)
                ContentListView.style.width = skillEditorConfig.frameUniWidth * CurrentFrameCount;
        }

        /// <summary>
        /// 初始化主要容器
        /// </summary>
        private void InitContent()
        {
            ContentListView = root.Q<VisualElement>("ContentListView");
            TrackMenuParent = root.Q<VisualElement>("TrackMenuList");
            MainContentView = root.Q<ScrollView>("MainContentView");
            MainContentView.verticalScroller.valueChanged += MainContentViewVerticalValueChanged;
            UpdateContentSise();
            InitTrack();
        }

        /// <summary>
        /// 初始化轨道
        /// </summary>
        private void InitTrack()
        {
            if (SkillConfig != null)
            {
                DestroyTracksView();

                foreach (KeyValuePair<string, SkillTrackDataBase> item in SkillConfig.trackDataDic)
                {
                    TrackBase track = Activator.CreateInstance(Type.GetType(item.Key)) as TrackBase;
                    string trackName = Type.GetType(item.Key).GetField("TrackName", BindingFlags.Static | BindingFlags.Public).GetValue(track).ToString();
                    if (track != null)
                    {
                        track.Init(TrackMenuParent, ContentListView, skillEditorConfig.frameUniWidth, item.Value, trackName);
                        trackList.Add(track);
                    }
                }
            }
            else
            {
                DestroyTracksView();
            }
        }

        /// <summary>
        /// 公开的InitTrack
        /// </summary>
        public void P_InitTrack()
        { InitTrack(); }

        #endregion 轨道

        #region 预览

        private bool isPlaying;

        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                if (isPlaying)
                {
                    startTime = DateTime.Now;
                    startFrameIndex = CurrentSelectFrameIndex;
                    for (int i = 0; i < trackList.Count; i++)
                    {
                        trackList[i].OnPlay(currentFrameCount);
                    }
                    EditorCoroutineUtility.StartCoroutine(SkillUpdate(), this);
                }
                else
                {
                    for (int i = 0; i < trackList.Count; i++)
                    {
                        trackList[i].OnStop();
                    }
                }
            }
        }

        private DateTime startTime;
        private int startFrameIndex;

        private IEnumerator SkillUpdate()
        {
            while (IsPlaying)
            {
                //时间差
                float time = (float)DateTime.Now.Subtract(startTime).TotalSeconds;
                //帧率
                float frameRate;
                if (skillConfig != null) { frameRate = skillConfig.FrameRate; }
                else { frameRate = skillEditorConfig.defaltFrameRote; }
                //计算当前帧
                CurrentSelectFrameIndex = (int)(time * frameRate * animationSpeed) + startFrameIndex;
                if (CurrentSelectFrameIndex == CurrentFrameCount)
                {
                    IsPlaying = false;
                }
                yield return null;
            }
            yield break;
        }

        /// <summary>
        /// 驱动技能表现
        /// </summary>
        private void TickSkill()
        {
            //驱动技能表现
            if (skillConfig != null && currentPreviewGameObject != null)
            {
                //驱动动画
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].TickView(CurrentSelectFrameIndex);
                }
            }
        }

        #endregion 预览

        /// <summary>
        /// 重置轨道数据
        /// </summary>
        private void ResetTrackData()
        {
            foreach (TrackBase item in trackList)
            {
                item.OnConfigChange();
            }
        }

        /// <summary>
        /// 保存Config配置刷新数据
        /// </summary>
        public void AutoSaveConfig()
        {
            if (skillEditorConfig.isAutoSaveConfig)
            {
                SaveConfig();
            }
        }

        /// <summary>
        /// 直接保存
        /// </summary>
        public void SaveConfig()
        {
            if (skillConfig != null)
            {
                EditorUtility.SetDirty(skillConfig);
                AssetDatabase.SaveAssetIfDirty(skillConfig);
                ResetTrackData();
            }
        }

        /// <summary>
        /// 删除所有轨道视图
        /// </summary>
        public void DestroyTracksView()
        {
            //删除所有轨道
            foreach (var item in trackList)
            {
                item.DestroyTrackView();
            }
            trackList.Clear();
        }

        public void ResetView(float frameWidth)
        {
            foreach (var item in trackList)
            {
                item.ResetView(frameWidth);
            }
        }

        public void StartCoroutin(IEnumerator objects)
        {
            EditorCoroutineUtility.StartCoroutine(objects, this);
        }

        public void UnSelectAll()
        {
            foreach (TrackBase item in trackList)
            {
                item.UnSelectAll();
            }
        }
    }
}