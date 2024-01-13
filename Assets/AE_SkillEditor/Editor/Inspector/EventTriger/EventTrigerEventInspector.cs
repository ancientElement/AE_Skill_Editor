using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Sirenix.Utilities;

namespace ARPG_AE_JOKER.SkillEditor
{
    [CustomEditor(typeof(EventTrigerEventInspectorHelper))]
    public class EventTrigerEventDataInspector : Editor
    {
        // 可排序的列表
        private ReorderableList reorderableList;

        public void OnEnable()
        {
            EventTrigerEventInspectorHelper helper = target as EventTrigerEventInspectorHelper;
            if (helper == null) return;

            //初始化列表
            reorderableList = new ReorderableList(helper.eventNames, typeof(string), true, true, true, true);
            reorderableList.onAddCallback += Add;
            reorderableList.onRemoveCallback += Remove;
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.onCanAddCallback += CanAddOrDelete;
        }

        private bool CanAddOrDelete(ReorderableList list)
        {
            return Application.isPlaying == true ? false : true;
        }

        //绘制事件列表
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EventTrigerEventInspectorHelper helper = target as EventTrigerEventInspectorHelper;
            if (helper == null) return;

            string name = helper.eventNames[index];

            var left_container = new Rect(rect.x, rect.y, rect.width * 0.5f, rect.height);
            var right_container = new Rect(left_container.x + left_container.width, left_container.y, rect.width * 0.5f, rect.height);

            //左半边
            EditorGUI.LabelField(left_container, "事件" + index);

            //右半边
            //确认之后再保存SO文件防止多次保存产生卡顿
            EditorGUI.BeginChangeCheck();
            //延迟赋值
            string tempname = EditorGUI.DelayedTextField(right_container, name);
            if (EditorGUI.EndChangeCheck())
            {
                helper.eventNames[index] = tempname;
                EditorUtility.SetDirty(helper.eventNameListSo);
                AssetDatabase.SaveAssetIfDirty(helper.eventNameListSo);
            }
        }

        //移除事件回调
        private void Remove(ReorderableList list)
        {
            EventTrigerEventInspectorHelper helper = target as EventTrigerEventInspectorHelper;
            if (helper == null) return;
            helper.eventNames.RemoveAt(list.index);
            EditorUtility.SetDirty(helper.eventNameListSo);
            AssetDatabase.SaveAssetIfDirty(helper.eventNameListSo);
        }

        //添加事件回调
        private void Add(ReorderableList list)
        {
            EventTrigerEventInspectorHelper helper = target as EventTrigerEventInspectorHelper;
            if (helper == null) return;
            helper.eventNames.Add("");
            EditorUtility.SetDirty(helper.eventNameListSo);
            AssetDatabase.SaveAssetIfDirty(helper.eventNameListSo);
        }

        /// <summary>
        /// 真的是艹了在 OnInspectorGUI 里面不能用 EditorGUI 只能用 EditorGUILayout 
        /// </summary>
        public override void OnInspectorGUI()
        {
            EventTrigerEventInspectorHelper helper = target as EventTrigerEventInspectorHelper;
            if (helper == null) return;

            EditorGUILayout.BeginVertical();
            //事件名
            EditorGUILayout.LabelField(new GUIContent("事件名"));

            System.Collections.Generic.List<string> list = helper.eventNames;

            for (int i = 0; i < helper.skillFrameEventBase.eventName.Count; i++)
            {
                string eventName = helper.skillFrameEventBase.eventName[i];
                int index = i;
                if (EditorGUILayout.DropdownButton(new GUIContent(eventName), FocusType.Keyboard))
                {
                    GenericMenu genericMenu = new GenericMenu();
                    //注册Menu的item
                    foreach (string item in list)
                    {
                        string name = item;
                        if (name == "") name = "未设置事件名称";
                        //设置内容、勾选条件和点击回调
                        genericMenu.AddItem(new GUIContent(name), item == eventName, () =>
                        {
                            Debug.Log(i);
                            helper.skillFrameEventBase.eventName[index] = item;
                            helper.OnValidate();
                        });
                    }
                    //显示菜单
                    genericMenu.ShowAsContext();
                }
            }

            EditorGUILayout.EndVertical();

            //颜色
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent("颜色"));
            helper.skillFrameEventBase.color = EditorGUILayout.ColorField(helper.skillFrameEventBase.color);
            EditorGUILayout.EndVertical();

            //事件列表
            EditorGUILayout.LabelField(new GUIContent("事件列表"));
            helper.eventNameListSo = (EventNameListSO)EditorGUILayout.ObjectField(helper.eventNameListSo, typeof(EventNameListSO), false);
            reorderableList.list = helper.eventNames;
            reorderableList.DoLayoutList();
        }
    }
}