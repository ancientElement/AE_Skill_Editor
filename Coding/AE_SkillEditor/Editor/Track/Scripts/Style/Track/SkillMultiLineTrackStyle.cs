using UnityEditor;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public partial class SkillMultiLineTrackStyle : SkillTrackStyleBase
    {
        #region 常量

        private const string MenuAssetpath = "Assets/AE_SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MuliLineTrackMenu.uxml";
        private const string TrackAssetpath = "Assets/AE_SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiiLineTrackContent.uxml";

        #endregion 常量

        public readonly float headHeight = 30f;//头部高度
        public readonly float itemHeight = 32f;//头部高度

        public VisualElement trackMenuItemParent;//菜单项的容器

        private Button FlodButton;

        public bool isFold;

        /// <summary>
        /// 初始化轨道
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

            menuParent.Add(menuRoot);
            contentParent.Add(contentRoot);

            titlelabel = menuRoot.Q<Label>("Title");
            titlelabel.text = title;

            //折叠子轨道按钮
            FlodButton = menuRoot.Q<Button>("FlodButton");
            FlodButton.clicked += () => { FlodOrOpenButtonClick(); };

            //菜单项的容器
            trackMenuItemParent = menuRoot.Q<VisualElement>("TrackMenuList");
        }

        public void FlodOrOpenButtonClick()
        {
            if (!isFold)
            {
                FlodChildTrack();
                FlodButton.text = "▲";
            }
            else
            {
                OpneChildTrack();
                FlodButton.text = "▼";
            }
        }

        //折叠子轨道
        public void FlodChildTrack()
        {
            if (trackMenuItemParent != null)
            {
                foreach (VisualElement item in trackMenuItemParent.Children())
                {
                    item.style.display = DisplayStyle.None;
                }
                foreach (VisualElement item in contentRoot.Children())
                {
                    item.style.display = DisplayStyle.None;
                }
                isFold = true;
            }
            UpdataSise(0);
        }

        //打开子轨道
        public void OpneChildTrack()
        {
            if (trackMenuItemParent != null)
            {
                foreach (VisualElement item in trackMenuItemParent.Children())
                {
                    item.style.display = DisplayStyle.Flex;
                }
                foreach (VisualElement item in contentRoot.Children())
                {
                    item.style.display = DisplayStyle.Flex;
                }
                isFold = false;
            }
            UpdataSise(trackMenuItemParent.childCount);
        }

        /// <summary>
        /// 添加子轨道
        /// </summary>
        /// <returns></returns>
        public ChildTrackStyle CreateChildTrackInTheEnd(int index)
        {
            ChildTrackStyle child = new ChildTrackStyle();
            child.Init(trackMenuItemParent, contentRoot, index, headHeight, itemHeight);
            return child;
        }

        /// <summary>
        /// 更新容器体积
        /// </summary>
        public void UpdataSise(int count)
        {
            float height = itemHeight * count + headHeight;
            contentRoot.style.height = height;
            menuRoot.style.height = height;

            trackMenuItemParent.style.height = itemHeight * count;
        }
    }
}