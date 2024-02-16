using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AE_Framework
{
    /// <summary>
    /// 自动找到控件并保存
    /// 提供给外部得到控件的方法
    /// </summary>
    public class BasePanel : MonoBehaviour
    {
        /// <summary>
        /// 用UIMgr 显示面板时自动调用
        /// </summary>
        public virtual void ShowMe(params object[] args)
        {
        }

        /// <summary>
        /// 用UIMgr 隐藏面板时自动调用
        /// </summary>
        public virtual void HindMe()
        {
        }

#if UNITY_EDITOR

        [ContextMenu("UI组件自动赋值/精确(物体名与变量名一致)")]
        public void FindControllers()
        {
            FindController<Button>();
            FindController<Image>();
            FindController<Text>();
            FindController<Toggle>();
            FindController<Slider>();
            FindController<ScrollRect>();
            FindController<InputField>();
        }

        private void FindController<T>() where T : UIBehaviour
        {
            Type type = GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            T[] controls = GetComponentsInChildren<T>();
            //寻找button组件并通过游戏对象名字储存
            for (int i = 0; i < controls.Length; i++)
            {
                foreach (var item in fieldInfos)
                {
                    if (item.Name == controls[i].gameObject.name && item.FieldType == typeof(T))
                    {
                        item.SetValue(this, controls[i]);
                    }
                }
            }
        }

#endif
    }
}