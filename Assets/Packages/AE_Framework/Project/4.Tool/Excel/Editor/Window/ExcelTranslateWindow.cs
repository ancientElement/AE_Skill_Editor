using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

namespace AE_Framework
{
    public class ExcelTranslateWindow : EditorWindow
    {
        [MenuItem("Tools/AE_Framework/ExcelTranslateWindow")]
        public static void ShowExample()
        {
            ExcelTranslateWindow wnd = GetWindow<ExcelTranslateWindow>();
            wnd.titleContent = new GUIContent("ExcelTranslateWindow");
        }

        VisualElement root;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AE_Framework/Project/4.Tool/Excel/Editor/Window/ExcelTranslateWindow.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/AE_Framework/Project/4.Tool/Excel/Editor/Window/ExcelTranslateWindow.uss");
            root.styleSheets.Add(styleSheet);

            InitElements();

            InitGenerate();
        }

        /// <summary>
        /// 总体布局
        /// </summary>
        VisualElement GenerateSimpleCSArea;

        Label ExcalPathDragArea;
        TextField CSDataPathTextField;
        TextField CSContainerDataPathTextField;
        TextField SODataPathTextField;
        IMGUIContainer ExcalPathsArea;
        Button GenerateSOContainerCSButton;
        Button GenerateSOCSButton;
        Button GenerateJSONCSButton;

        /// <summary>
        /// 初始化元素
        /// </summary>
        private void InitElements()
        {
            //总体布局
            GenerateSimpleCSArea = root.Q<VisualElement>(nameof(GenerateSimpleCSArea));

            ExcalPathDragArea = root.Q<Label>(nameof(ExcalPathDragArea));
            CSDataPathTextField = root.Q<TextField>(nameof(CSDataPathTextField));
            SODataPathTextField = root.Q<TextField>(nameof(SODataPathTextField));
            CSContainerDataPathTextField = root.Q<TextField>(nameof(CSContainerDataPathTextField));
            ExcalPathsArea = root.Q<IMGUIContainer>(nameof(ExcalPathsArea));
            GenerateSOContainerCSButton = root.Q<Button>(nameof(GenerateSOContainerCSButton));
            GenerateSOCSButton = root.Q<Button>(nameof(GenerateSOCSButton));
            GenerateJSONCSButton = root.Q<Button>(nameof(GenerateJSONCSButton));
        }

        #region 生成区域

        List<string> excelPaths = new List<string>();
        private ReorderableList reorderableList;

        public void InitGenerate()
        {
            excelPaths.Clear();
            CSDataPathTextField.value = ExcelTool.DATA_ClASS_PATH;
            CSContainerDataPathTextField.value = ExcelTool.DATA_CONTAINER_PATH;
            SODataPathTextField.value = ExcelTool.SO_Data_Path;

            ExcalPathDragArea.RegisterCallback<DragUpdatedEvent>(ExcalPathDragUpdateEvent);
            ExcalPathDragArea.RegisterCallback<DragExitedEvent>(ExcalPathDragDragExitedEvent);

            ExcalPathsArea.onGUIHandler = ExcalPathsAreaOnGUI;

            GenerateSOContainerCSButton.clicked += GenerateSOContainerCSButton_clicked;
            GenerateSOCSButton.clicked += GenerateSOCSButton_clicked;
            GenerateJSONCSButton.clicked += GenerateJSONContainerCSButton_clicked;
        }

        private void GenerateJSONContainerCSButton_clicked()
        {
            foreach (string item in excelPaths)
            {
                ExcelTool.GenerateJson(new FileInfo(item));
            }
        }

        private void GenerateSOCSButton_clicked()
        {
            foreach (string item in excelPaths)
            {
                ExcelTool.GenerateExcelSO(new FileInfo(item));
            }
        }

        private void GenerateSOContainerCSButton_clicked()
        {
            foreach (string item in excelPaths)
            {
                ExcelTool.GenarateExcel(new FileInfo(item));
            }
        }

        private void ExcalPathDragUpdateEvent(DragUpdatedEvent evt)
        {
            string[] paths = DragAndDrop.paths;
            string fileName;
            foreach (string item in paths)
            {
                fileName = item.Split('/').Last();
                if (fileName.EndsWith(".xlsx") || fileName.EndsWith(".xls"))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
            }
        }

        private void ExcalPathDragDragExitedEvent(DragExitedEvent evt)
        {
            string[] paths = DragAndDrop.paths;
            string fileName;
            foreach (string item in paths)
            {
                fileName = item.Split('/').Last();
                if (fileName.EndsWith(".xlsx") || fileName.EndsWith(".xls"))
                {
                    AddSimpleCSPath(item);
                }
            }
        }

        private void ExcalPathsAreaOnGUI()
        {
            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(excelPaths, typeof(List<string>), true, true, false, true);

                reorderableList.onRemoveCallback += RemovePath;
            }
            reorderableList.list = excelPaths;

            reorderableList.DoLayoutList();
        }

        private void AddSimpleCSPath(string path)
        {
            if (excelPaths.Contains(path)) { Debug.LogWarning("已存在该文件"); return; }
            excelPaths.Add(path);
        }

        private void RemovePath(ReorderableList list)
        {
            excelPaths.RemoveAt(list.index);
        }

        #endregion
    }
}