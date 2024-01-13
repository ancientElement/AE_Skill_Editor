using UnityEditor;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SkillSingleLineTrackStyle : SkillTrackStyleBase
    {
        private const string MenuAssetpath = "Assets/AE_SkillEditor/Editor/Track/Assets/SingleLineTrackStyle/SingleLineTrackMenu.uxml";
        private const string TrackAssetpath = "Assets/AE_SkillEditor/Editor/Track/Assets/SingleLineTrackStyle/SingleLineContent.uxml";

        /// <summary>
        /// 初始化一条轨道
        /// </summary>
        /// <param name="menueparent"></param>
        /// <param name="contentParent"></param>
        /// <param name="title"></param>
        public void Init(VisualElement menueparent, VisualElement contentParent, string title)
        {
            this.menuParent = menueparent;
            this.contentParent = contentParent;

            this.menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetpath).Instantiate().Query().ToList()[1];
            this.contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetpath).Instantiate().Query().ToList()[1];

            titlelabel = (Label)menuRoot;
            titlelabel.text = title;

            menuParent.Add(menuRoot);
            contentParent.Add(contentRoot);
        }
    }
}