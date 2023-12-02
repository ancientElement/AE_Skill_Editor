using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AE_Framework
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "AE_Framework/Config/GameSetting")]
    public class GameSettings : SerializedScriptableObject
    {
        // public enum AssetLoadMethod
        // {
        //     Addressables,
        //     Reaourses
        // }
        //
        // [Title("资源加载")]
        // [LabelText("资源加载方式")]
        // public AssetLoadMethod assetLoadMethod = AssetLoadMethod.Addressables;

        [Title("UI窗口设置"), LabelText("窗口信息")]
        [DictionaryDrawerSettings(KeyLabel = "类型", ValueLabel = "UI窗口数据")]
        public Dictionary<Type, UIElement> UIElementDic = new Dictionary<Type, UIElement>();

#if UNITY_EDITOR

        [Button(Name = "初始化游戏配置", ButtonHeight = 50)]
        [GUIColor(0, 1, 0)]
        public void InitForEditor()
        {
            UIElementAttributeOnEditor();
        }

        private void UIElementAttributeOnEditor()
        {
            UIElementDic.Clear();
            // 获取所有程序集
            System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            Type baseType = typeof(BasePanel);
            // 遍历程序集
            foreach (System.Reflection.Assembly assembly in asms)
            {
                // 遍历程序集下的每一个类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (baseType.IsAssignableFrom(type)
                        && !type.IsAbstract)
                    {
                        UIElementAttribute attribute = type.GetCustomAttribute<UIElementAttribute>();
                        if (attribute != null)
                        {
                            UIElementDic.Add(type, new UIElement()
                            {
                                isCache = attribute.isCache,
                                prefabAssetName = attribute.prefabAssetName,
                                layerNum = attribute.layerNum
                            });
                        }
                    }
                }
            }
        }

#endif
    }
}