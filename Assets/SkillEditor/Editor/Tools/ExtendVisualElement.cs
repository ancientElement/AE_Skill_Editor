using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

public static class ExtendVisualElement
{
    /// <summary>
    /// 添加右键菜单
    /// </summary>
    /// <param name="element"></param>
    /// <param name="actionName">项目名</param>
    /// <param name="action">点击回调</param>
    /// <param name="status"></param>
    public static void AddOneRightMenue(this VisualElement element, string actionName, Action<DropdownMenuAction> action, DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal)
    {
        ContextualMenuManipulator m = new ContextualMenuManipulator((ContextualMenuPopulateEvent contextEvent) =>
        {
            MyDelegate(contextEvent, actionName, action, status);
        });
        m.target = element;
    }

    private static void MyDelegate(ContextualMenuPopulateEvent contextEvent, string actionName, Action<DropdownMenuAction> action, DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal)
    {
        contextEvent.menu.AppendAction(actionName, action, status);
    }

    /// <summary>
    /// 改变光标样式
    /// </summary>
    /// <param name="element"></param>
    /// <param name="cursor"></param>
    public static void SetCursor(this VisualElement element, MouseCursor cursor)
    {
        object objCursor = new UnityEngine.UIElements.Cursor();
        PropertyInfo fields = typeof(UnityEngine.UIElements.Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance);
        fields.SetValue(objCursor, (int)cursor);
        element.style.cursor = new StyleCursor((UnityEngine.UIElements.Cursor)objCursor);
    }
}