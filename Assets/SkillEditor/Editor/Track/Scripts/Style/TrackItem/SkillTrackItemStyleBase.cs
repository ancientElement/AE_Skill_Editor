using Sirenix.Utilities;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public abstract class SkillTrackItemStyleBase
    {
        public VisualElement root { get; protected set; }//根节点

        public SkillTrackStyleBase m_ParentTrackStyle;//父级样式

        public virtual void Init(SkillTrackStyleBase m_ParentTrackStyle, float width, float position, string name, string TrackItemAssetPAth)
        {
            root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackItemAssetPAth).Instantiate().Query<Label>();

            this.m_ParentTrackStyle = m_ParentTrackStyle;
            this.m_ParentTrackStyle.AddItem(root);

            RsetView(name, position, width);
        }

        /// <summary>
        /// 寻找元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public VisualElement FindElement<T>(string name) where T : VisualElement
        {
            return root.Q<T>(name);
        }

        /// <summary>
        /// 重置视图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="x"></param>
        public virtual void RsetView(string name, float x, float width)
        {
            //名字
            SetName(name);
            //位置
            SetPosition(x);
            //宽度
            SetWidth(width);
        }

        public virtual void Destory()
        {
            m_ParentTrackStyle.DeleteItem(root);
        }

        public virtual void SetName(string name)
        { root.Q<Label>().text = name; }

        public virtual void SetBGColor(Color color)
        { root.style.backgroundColor = color; }

        public virtual void SetWidth(float width)
        { root.style.width = width; }

        public virtual void SetPosition(float position)
        {
            Vector3 pos = root.transform.position;
            pos.x = position;
            root.transform.position = pos;
        }
    }

    public class SimpleItemStyle : SkillTrackItemStyleBase
    {
        public override void Init(SkillTrackStyleBase ParentTrackStyle, float width, float position, string name, string TrackItemAssetPAth = null)
        {
            TrackItemAssetPAth = TrackItemAssetPAth == null ? "Assets/SkillEditor/Editor/Track/Assets/TrackItem/AnimationTrackItem.uxml" : TrackItemAssetPAth;
            base.Init(ParentTrackStyle, width, position, name, TrackItemAssetPAth);
        }
    }

    public class EventItemStyle : SimpleItemStyle
    {
        IMGUIContainer iMGUIContainer;
        Color iMGUIColor;

        public override void Init(SkillTrackStyleBase ParentTrackStyle, float width, float position, string name, string TrackItemAssetPAth = null)
        {
            base.Init(ParentTrackStyle, width, position, name, "Assets/SkillEditor/Editor/Track/Assets/TrackItem/EvenItemStyle.uxml");
            iMGUIContainer = root.Q<IMGUIContainer>("EventItemIMGUIContainer");
            iMGUIContainer.onGUIHandler = DrawEventItem;
        }

        private void DrawEventItem()
        {
            Handles.BeginGUI();

            Handles.color = iMGUIColor;
            Rect rect = iMGUIContainer.contentRect;

            //Handles.DrawLine(new Vector3(rect.x, 0.1f), new Vector3(rect.x + rect.width, 0.1f));

            //绘制事件标签
            Vector3[] list = new Vector3[5] {
              new Vector3(rect.width*0.5f,0),
              new Vector3(rect.width,rect.width * 0.5f),
              new Vector3(rect.width,rect.height),
              new Vector3(0,rect.height),
              new Vector3(0,rect.width* 0.5f)
            };
            Handles.DrawAAConvexPolygon(list);

            Handles.EndGUI();
        }

        public override void SetBGColor(Color color)
        {
            //base.SetBGColor(color);
            iMGUIColor = color;
        }

        public override void SetName(string name)
        {
            //base.SetName(name);
            root.Q<Label>("EventNameLabel").text = name;
        }
    }
}