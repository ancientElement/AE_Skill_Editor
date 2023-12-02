//using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AE_Framework
{
    /// <summary>
    /// 显示面板
    /// 隐藏面板
    /// 自定义事件监听
    /// </summary>
    public class UIMgr : SingletonMonoMgr<UIMgr>
    {
        private Dictionary<Type, UIElement> UIElementDic => GameRoot.Instance.GameSetting.UIElementDic;

        /// <summary>
        /// 将canvas提供给外部
        /// </summary>
        //[LabelText("UI根节点")]
        [SerializeField] public RectTransform UI_Root;

        //LabelText("UI层级")
        [SerializeField] private Transform[] Layers;

        /// <summary>
        /// 显示面板
        /// </summary>
        /// <typeparam name="T">面板的组件</typeparam>
        /// <param name="layer">面板处在哪一个层级</param>
        /// <param name="callback">是否需要回调</param>
        /// <param name="isFull">是否是全屏面板</param>
        public void ShowPanel<T>(bool isFull = false, int layer = -1, params object[] args) where T : BasePanel
        {
            if (UIElementDic.ContainsKey(typeof(T)))
            {
                UIElement info = UIElementDic[typeof(T)];
                int layerNum = layer == -1 ? info.layerNum : layer;
                if (info.objInstance != null)
                {
                    info.objInstance.gameObject.SetActive(true);
                    info.objInstance.transform.SetParent(Layers[layerNum]);
                    info.objInstance.transform.SetAsLastSibling();
                }
                else
                {
                    string name = info.prefabAssetName;
                    var res = ResMgr.AddressableLoad<GameObject>(name);
                    InitPanel(info, res, isFull, args);
                }
            }
        }

        /// <summary>
        /// 初始化位置缩放
        /// </summary>
        /// <param name="uIElement"></param>
        /// <param name="res"></param>
        /// <param name="isFull"></param>
        private void InitPanel(UIElement uIElement, GameObject res, bool isFull = false, params object[] args)
        {
            res.transform.SetParent(Layers[uIElement.layerNum], false);
            //得到面板脚本
            uIElement.objInstance = res.GetComponent<BasePanel>();
            //调用showMe
            uIElement.objInstance.ShowMe(args);
            RectTransform tf = res.GetComponent<RectTransform>();
            //设置位置 缩放 偏移
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
            if (isFull)
            {
                tf.offsetMax = Vector3.zero;
                tf.offsetMin = Vector3.zero;
            }
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="panelName"></param>
        public void HidePanel<T>()
        {
            if (UIElementDic.ContainsKey(typeof(T)))
            {
                if (UIElementDic[typeof(T)].objInstance == null) return;

                //调用面板的hindme
                UIElementDic[typeof(T)].objInstance?.HindMe();
                ResMgr.ReleaseInstance(UIElementDic[typeof(T)].objInstance.gameObject);
            }
        }

        /// <summary>
        /// 得到面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public T GetPanel<T>() where T : BasePanel
        {
            if (UIElementDic.ContainsKey(typeof(T)))
                return UIElementDic[typeof(T)].objInstance as T;
            return null;
        }

        /// <summary>
        /// 添加自定义事件 EventTrigger
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type"></param>
        /// <param name="callbak"></param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,
            UnityAction<BaseEventData> callbak)
        {
            //创建 EventTrigger 组件
            EventTrigger eventTrigger = control.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = control.AddComponent<EventTrigger>();

            //为EventTrigger 组件添加事件
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type; //指定事件类型
            entry.callback.AddListener(callbak); //指定事件回调
            eventTrigger.triggers.Add(entry); //添加事件进EventTrigger
        }
    }
}