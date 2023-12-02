using System;
using System.Collections.Generic;
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
        private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

        /// <summary>
        /// 寻找到所有控件并储存
        /// </summary>
        protected void FindAllControl()
        {
            FindControl<Button>();
            FindControl<Image>();
            FindControl<Text>();
            FindControl<Toggle>();
            FindControl<Slider>();
            FindControl<ScrollRect>();
            FindControl<InputField>();
        }

        /// <summary>
        /// 重写此方法判断哪个按钮被点击
        /// 并作出相应相应
        /// </summary>
        /// <param name="btnName">按钮名字</param>
        protected virtual void OnColick(string btnName)
        {
        }

        /// <summary>
        /// 重写此方法判断哪个单选框被点击
        /// </summary>
        /// <param name="toggleName"></param>
        /// <param name="value"></param>
        protected virtual void onValueChanged(string toggleName, bool value)
        {
        }

        /// <summary>
        /// 寻找控件通过游戏对象名储存游戏对象身上的一系列控件
        /// 并添加事件监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindControl<T>() where T : UIBehaviour
        {
            T[] controls = GetComponentsInChildren<T>();
            //寻找button组件并通过游戏对象名字储存
            for (int i = 0; i < controls.Length; i++)
            {
                string objName = controls[i].gameObject.name;
                //有游戏名的控件组
                //加入这个控件
                if (controlDic.ContainsKey(objName))
                    controlDic[objName].Add(controls[i]);
                //没有创建游戏名的控件组
                //并初始化时加入
                else
                    controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });

                //添加事件监听
                //按钮
                if (controls[i] is Button)
                {
                    //用匿名函数调用有参行数
                    (controls[i] as Button).onClick.AddListener(() =>
                    {
                        OnColick(objName);
                    });
                }
                //单选框
                if (controls[i] is Toggle)
                {
                    //用匿名函数调用有参行数
                    (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                    {
                        onValueChanged(objName, value);
                    });
                }
            }
        }

        /// <summary>
        /// 向外部提供控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlName"></param>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (controlDic.ContainsKey(controlName))
                for (int i = 0; i < controlDic[controlName].Count; i++)
                {
                    if (controlDic[controlName][i] is T)
                        return controlDic[controlName][i] as T;
                };
            return null;
        }

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
    }
}