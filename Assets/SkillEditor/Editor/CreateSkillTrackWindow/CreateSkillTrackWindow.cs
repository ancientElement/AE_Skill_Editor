using ARPG_AE_JOKER;
using ARPG_AE_JOKER.SkillEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateSkillTrackWindow : EditorWindow
{
    private VisualElement root;

    public static CreateSkillTrackWindow Instance { get; private set; }

    [MenuItem("Window/UI Toolkit/CreateSkillTrackWindow")]
    public static void ShowExample()
    {
        CreateSkillTrackWindow wnd = GetWindow<CreateSkillTrackWindow>();
        wnd.titleContent = new GUIContent("创建轨道");
    }

    public void CreateGUI()
    {
        root = rootVisualElement;

        Instance = this;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SkillEditor/Editor/CreateSkillTrackWindow/CreateSkillTrackWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
    }

    private SkillConfig skillConfig;//配置文件

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="skillConfig"></param>
    public void Init(SkillConfig skillConfig)
    {
        this.skillConfig = skillConfig;
        InitAddAndDelete();
        InitCreate();
    }

    #region 新建轨道

    private DropdownField CreateTrackMultiOrSingleDropdownField;
    private TextField CreateTrackNameCH;
    private TextField CreateTrackNameCS;
    private Label CreateTrackClassPath;
    private TextField CreateEventNameCS;
    private TextField ObejcTypeTextFiled;
    private TextField ObejcNameTextFiled;
    private Label InspectorPathCS;
    private Label EventDataPathCS;
    private Button CreateNewTrackButton;
    private ColorField CreateNormalColor;
    private ColorField CreateSelectColor;

    //模板地址
    public static string MultiLineTrackPath = "Assets/SkillEditor/Editor/Track/Assets/MobaTrack/TestMultiTrack.txt";

    public static string MultiLineTrackEventPath = "Assets/SkillEditor/Editor/Track/Assets/MobaTrack/TestMultiLineFrameEven.txt";
    public static string SingleLineTrackPath = "Assets/SkillEditor/Editor/Track/Assets/MobaTrack/TestSingLineTrack.txt";
    public static string SingleLineTrackEventPath = "Assets/SkillEditor/Editor/Track/Assets/MobaTrack/TestSinglineEvent.txt";
    public static string InspecterHelperPath = "Assets/SkillEditor/Editor/Track/Assets/MobaTrack/TestEventInspectorHelper.txt";
    public static string InspecterPath = "Assets/SkillEditor/Editor/Track/Assets/MobaTrack/TestEventInspector.txt";

    public static string CoustomTrackCSPath = "Assets/SkillEditor_Coustom/Editor/Tracks/";
    public static string CoustomInspectorCSPath = "Assets/SkillEditor_Coustom/Editor/Inspector/";
    public static string CoustomEventCSPath = "Assets/SkillEditor_Coustom/RunTime/Configs/";

    private void InitCreate()
    {
        //Debug.Log(typeof(Texture).FullName);
        CreateTrackMultiOrSingleDropdownField = root.Q<DropdownField>("CreateTrackMultiOrSingleDropdownField");
        CreateTrackNameCH = root.Q<TextField>("CreateTrackNameCH");

        CreateTrackNameCS = root.Q<TextField>("CreateTrackNameCS");
        CreateTrackClassPath = root.Q<Label>("CreateTrackClassPath");

        CreateEventNameCS = root.Q<TextField>("CreateEventNameCS");
        ObejcTypeTextFiled = root.Q<TextField>("ObejcTypeTextFiled");
        ObejcNameTextFiled = root.Q<TextField>("ObejcNameTextFiled");
        EventDataPathCS = root.Q<Label>("EventDataPathCS");

        CreateNormalColor = root.Q<ColorField>("CreateNormalColor");
        CreateSelectColor = root.Q<ColorField>("CreateSelectColor");

        CreateNewTrackButton = root.Q<Button>("CreateNewTrackButton");

        InspectorPathCS = root.Q<Label>("InspectorPathCS");

        CreateTrackMultiOrSingleDropdownField.choices = new List<string>() { "单行轨道", "多行轨道" };

        CreateTrackNameCS.RegisterValueChangedCallback(TrackNameCSValueChanged);
        CreateEventNameCS.RegisterValueChangedCallback(CreateEventNameCSValueChanged);

        CreateNewTrackButton.clicked += CreateNewTrackClick;
    }

    private void CreateEventNameCSValueChanged(ChangeEvent<string> evt)
    {
        if (evt.previousValue != evt.newValue)
        {
            EventDataPathCS.text = CoustomTrackCSPath + evt.newValue + ".cs";
            InspectorPathCS.text = CoustomInspectorCSPath + evt.newValue + "Inspector" + ".cs";
        }
    }

    private void TrackNameCSValueChanged(ChangeEvent<string> evt)
    {
        if (evt.previousValue != evt.newValue)
        {
            CreateTrackClassPath.text = CoustomEventCSPath + evt.newValue + "Track.cs";
        }
    }

    private void CreateNewTrackClick()
    {
        string srtTrack = string.Empty;
        string srtEvent = string.Empty;
        string srtInspector = string.Empty;
        string srtInspectorHelper = string.Empty;
        if (CreateTrackMultiOrSingleDropdownField.value == "多行轨道")
        {
            srtTrack = File.ReadAllText(MultiLineTrackPath);
            srtEvent = File.ReadAllText(MultiLineTrackEventPath);
        }
        else if (CreateTrackMultiOrSingleDropdownField.value == "单行轨道")
        {
            srtTrack = File.ReadAllText(SingleLineTrackPath);
            srtEvent = File.ReadAllText(SingleLineTrackEventPath);
        }
        else
        {
            Debug.LogWarning("未选择轨道类型");
            return;
        }
        srtInspector = File.ReadAllText(InspecterPath);
        srtInspectorHelper = File.ReadAllText(InspecterHelperPath);

        string TrackNameCH = CreateTrackNameCH.value;//_轨道名称中文
        string TrackNameCS = CreateTrackNameCS.value + "Track";//_轨道名称CS
        string TrackChildTrackNameCS = CreateTrackNameCS.value + "ChildTrack";//_多行轨道子轨道名称CS
        string TrackTrackItemNameCS = CreateTrackNameCS.value + "TrackItem";//_多行轨道子轨道的单个Item名称CS
        string TrackEventNameCS = CreateEventNameCS.value;//_Event类名CS
        string ObjectTypeName = ObejcTypeTextFiled.value;//_变化的对象类型
        string ObejcName = ObejcNameTextFiled.value;//_变化的对象名
        string NormalColor = $"{CreateNormalColor.value.r}f,{CreateNormalColor.value.g}f,{CreateNormalColor.value.b}f,{CreateNormalColor.value.a}f";//_Item的普通状态颜色
        string SelectColor = $"{CreateSelectColor.value.r}f,{CreateSelectColor.value.g}f,{CreateSelectColor.value.b}f,{CreateSelectColor.value.a}f"; ;//_Item的选中状态颜色

        if (CreateTrackNameCH.value == "" || CreateTrackNameCS.value == "" || CreateEventNameCS.value == "" || ObejcTypeTextFiled.value == "" || ObejcNameTextFiled.value == "")
        {
            Debug.LogWarning("有必填项未填");
            return;
        }

        #region 替换

        srtTrack = srtTrack.Replace("_轨道名称中文", TrackNameCH);
        srtTrack = srtTrack.Replace("_轨道名称CS", TrackNameCS);
        srtTrack = srtTrack.Replace("_多行轨道子轨道名称CS", TrackChildTrackNameCS);
        srtTrack = srtTrack.Replace("_多行轨道子轨道的单个Item名称CS", TrackTrackItemNameCS);
        srtTrack = srtTrack.Replace("_Event类名CS", TrackEventNameCS);
        srtTrack = srtTrack.Replace("_变化的对象类型", ObjectTypeName);
        srtTrack = srtTrack.Replace("_变化的对象名", ObejcName);
        srtTrack = srtTrack.Replace("_Item的普通状态颜色", NormalColor);
        srtTrack = srtTrack.Replace("_Item的选中状态颜色", SelectColor);

        srtEvent = srtEvent.Replace("_轨道名称中文", TrackNameCH);
        srtEvent = srtEvent.Replace("_轨道名称CS", TrackNameCS);
        srtEvent = srtEvent.Replace("_多行轨道子轨道名称CS", TrackChildTrackNameCS);
        srtEvent = srtEvent.Replace("_多行轨道子轨道的单个Item名称CS", TrackTrackItemNameCS);
        srtEvent = srtEvent.Replace("_Event类名CS", TrackEventNameCS);
        srtEvent = srtEvent.Replace("_变化的对象类型", ObjectTypeName);
        srtEvent = srtEvent.Replace("_变化的对象名", ObejcName);

        srtInspector = srtInspector.Replace("_Event类名CS", TrackEventNameCS);
        srtInspectorHelper = srtInspectorHelper.Replace("_Event类名CS", TrackEventNameCS);

        #endregion 替换

        CreateFile(CoustomTrackCSPath, TrackNameCS + ".cs", srtTrack);
        CreateFile(CoustomEventCSPath, TrackEventNameCS + ".cs", srtEvent);
        CreateFile(CoustomInspectorCSPath, TrackEventNameCS + "Inspector" + ".cs", srtInspector);
        CreateFile(CoustomInspectorCSPath, TrackEventNameCS + "InspectorHelper" + ".cs", srtInspectorHelper);
    }

    private void CreateFile(string path, string fileName, string file)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText(path + fileName, file);
        AssetDatabase.Refresh();
    }

    #endregion 新建轨道

    #region 添加和删除轨道

    private Button AddTrackButton;//添加按钮
    private Button DeleteTrackButton;//删除按钮
    private DropdownField SelectTrackDropdownField;//添加选择框
    private DropdownField SelectDeleteTrackDropdownField;//删除选择框

    public List<string> tracks = new List<string>();

    /// <summary>
    /// 初始化 添加和删除轨道
    /// </summary>
    private void InitAddAndDelete()
    {
        GetAllTracksInAssmbly();

        AddTrackButton = root.Q<Button>(nameof(AddTrackButton));
        DeleteTrackButton = root.Q<Button>(nameof(DeleteTrackButton));
        SelectTrackDropdownField = root.Q<DropdownField>(nameof(SelectTrackDropdownField));
        SelectDeleteTrackDropdownField = root.Q<DropdownField>(nameof(SelectDeleteTrackDropdownField));

        SelectTrackDropdownField.choices = new List<string>();
        SelectDeleteTrackDropdownField.choices = new List<string>();

        //初始化选择性
        foreach (var item in tracks)
        {
            SelectTrackDropdownField.choices.Add(item);
            SelectDeleteTrackDropdownField.choices.Add(item);
        }
        AddTrackButton.clicked += AddTrackButtonClick;
        DeleteTrackButton.clicked += DeleteTrackButtonClick;
    }

    /// <summary>
    /// 获取所有轨道
    /// </summary>
    private void GetAllTracksInAssmbly()
    {
        // 获取所有程序集
        System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
        Type baseSingleLine = typeof(SingleLineTrack);
        Type baseMultiLine = typeof(MultiLineTrack);
        // 遍历程序集
        foreach (System.Reflection.Assembly assembly in asms)
        {
            // 遍历程序集下的每一个类型
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (typeof(SingleLineTrack<,>).IsAssignableFrom(type)
                   || typeof(MultiLineTrack<,>).IsAssignableFrom(type) || typeof(SingleLineTrack<>).IsAssignableFrom(type) || typeof(SingleLineTrack) == (type) || typeof(MultiLineTrack) == (type))
                {
                    //DO Nothing
                    //Debug.Log("BASE");
                }
                else if (baseSingleLine.IsAssignableFrom(type)
                    && !type.IsAbstract)
                {
                    //Debug.Log(type.FullName);
                    tracks.Add(type.FullName);
                }
                else if (baseMultiLine.IsAssignableFrom(type)
                    && !type.IsAbstract)
                {
                    //Debug.Log(type.FullName);
                    tracks.Add(type.FullName);
                }
            }
        }
    }

    /// <summary>
    /// 删除轨道
    /// </summary>
    private void DeleteTrackButtonClick()
    {
        string currentType = SelectDeleteTrackDropdownField.value;
        if (currentType != null)
        {
            if (skillConfig.trackDataDic.Keys.Contains(currentType))
            {
                skillConfig.RemoveTrack(currentType);
                Debug.LogWarning("已移除该轨道!!!" + currentType);
                SkillEditorWindow.Instance.P_InitTrack();
                return;
            }
            else
            {
                Debug.LogWarning("尚未创建该轨道!!!" + currentType);
            }
        }
    }

    /// <summary>
    /// 添加轨道
    /// </summary>
    public void AddTrackButtonClick()
    {
        string currentType = SelectTrackDropdownField.value;
        if (currentType != null)
        {
            Debug.Log(SelectTrackDropdownField.value);
            if (skillConfig.trackDataDic.Keys.Contains(currentType))
            {
                Debug.LogWarning("已有该轨道!!!请选择其他轨道");
                return;
            }
            if (tracks.Contains(currentType))
            {
                Type data = Type.GetType(currentType).GetField("m_Data", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).FieldType;
                skillConfig.AddTrack(currentType, (SkillTrackDataBase)Activator.CreateInstance(data));
                SkillEditorWindow.Instance.P_InitTrack();
            }
        }
    }

    #endregion 添加和删除轨道
}