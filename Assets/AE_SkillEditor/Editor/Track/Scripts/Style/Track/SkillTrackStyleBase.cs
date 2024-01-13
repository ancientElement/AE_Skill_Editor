using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public abstract class SkillTrackStyleBase
    {
        public Label titlelabel;

        //父级节点
        public VisualElement menuParent;

        public VisualElement contentParent;

        //自身节点
        public VisualElement menuRoot;

        public VisualElement contentRoot;

        public virtual void AddItem(VisualElement visualElement)
        {
            contentRoot.Add(visualElement);
        }

        public virtual void DeleteItem(VisualElement visualElement)
        {
            if (visualElement != null)
                contentRoot.Remove(visualElement);
        }

        public virtual void DestroyView()
        {
            if (menuRoot != null)
                menuParent.Remove(menuRoot);
            if (contentRoot != null)
                contentParent.Remove(contentRoot);
        }
    }
}