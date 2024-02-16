using UnityEngine;
using UnityEngine.EventSystems;

namespace AE_Framework
{
    /// <summary>
    /// 可拖拽,面板基类
    /// </summary>
    public class BaseGragablePanel : BasePanel, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Vector2 offset;

        public virtual void OnDrag(PointerEventData eventData)
        {
            (transform as RectTransform).position = eventData.position - offset;
            transform.SetAsLastSibling();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform,
                        Input.mousePosition, (eventData as PointerEventData).pressEventCamera, out offset);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}