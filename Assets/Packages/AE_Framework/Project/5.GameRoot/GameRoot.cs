using System;
using System.Collections.Generic;
using System.Reflection;

namespace AE_Framework
{
    public class GameRoot : SingletonMonoMgr<GameRoot>
    {
        public Dictionary<Type, UIElement> UIElementDic;

        public override void Init()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            base.Init();
            // 初始化所有管理器
            InitManager();
        }

        private void InitManager()
        {
            EventCenter.Clear();
            PoolMgr.Clear();
            UIElementAttribute();
            SingletonMonoMgr[] managers = GetComponents<SingletonMonoMgr>();
            for (int i = 0; i < managers.Length; i++)
            {
                if (managers[i] == this) continue;
                managers[i].Init();
            }
        }

        private void UIElementAttribute()
        {
            UIElementDic = new Dictionary<Type, UIElement>();
            UIElementDic.Clear();
            // 获取所有程序集
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            Type baseType = typeof(BasePanel);
            // 遍历程序集
            foreach (Assembly assembly in asms)
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
                                prefabAssetName = attribute.prefabAssetName,
                                layerNum = attribute.layerNum
                            });
                        }
                    }
                }
            }
        }
    }
}