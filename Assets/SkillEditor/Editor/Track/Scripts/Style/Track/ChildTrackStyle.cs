using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    /// <summary>
    /// 子轨道
    /// </summary>
    public class ChildTrackStyle : SkillTrackStyleBase
    {
        private const string childTrackMenuItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackMenuItem.uxml";//菜单
        private const string childTrackContentAssetItemPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiiLineTrackContentItem.uxml";//容器

        private TextField trackNametextField;//轨道名

        public void Init(VisualElement menueparent, VisualElement contentParent, int index, float headHeight, float itemHeight)
        {
            this.menuParent = menueparent;
            this.contentParent = contentParent;

            this.menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackMenuItemAssetPath).Instantiate().Query().ToList()[1];
            this.contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackContentAssetItemPath).Instantiate().Query().ToList()[1];

            menuParent.Add(menuRoot);
            contentParent.Add(contentRoot);

            trackNametextField = menuRoot.Q<TextField>("NameField");

            SetPosition(index, headHeight, itemHeight);
        }

        /// <summary>
        /// 设置当前轨道位置
        /// </summary>
        /// <param name="index"></param>
        public void SetPosition(int index, float headHeight, float itemHeight)
        {
            Vector3 pos = menuRoot.transform.position;
            pos.y = itemHeight * index;
            menuRoot.transform.position = pos;

            pos = contentRoot.transform.position;
            pos.y = itemHeight * index + headHeight;
            contentRoot.transform.position = pos;
        }

        /// <summary>
        /// 设置名字
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            trackNametextField.value = name;
        }
    }
}