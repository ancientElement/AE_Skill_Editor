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

        public void OnDrag(PointerEventData eventData)
        {
            (transform as RectTransform).position = eventData.position - offset;
            transform.SetAsLastSibling();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform,
                        Input.mousePosition, (eventData as PointerEventData).pressEventCamera, out offset);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}