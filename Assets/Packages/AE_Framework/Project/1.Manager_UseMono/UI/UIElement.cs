namespace AE_Framework
{
    /// <summary>
    /// UI元素
    /// </summary>
    public class UIElement
    {
        //[LabelText("预制体路径")]
        public string prefabAssetName;

        //[LabelText("UI层级")]
        public int layerNum;

        /// <summary>
        /// 这个元素的窗口对象
        /// </summary>
        public BasePanel objInstance;
    }
}