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

        public void SetName(string name)
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
        public override void Init(SkillTrackStyleBase ParentTrackStyle, float width, float position, string name, string TrackItemAssetPAth = "Assets/SkillEditor/Editor/Track/Assets/TrackItem/AnimationTrackItem.uxml")
        {
            base.Init(ParentTrackStyle, width, position, name, TrackItemAssetPAth);
        }
    }
}