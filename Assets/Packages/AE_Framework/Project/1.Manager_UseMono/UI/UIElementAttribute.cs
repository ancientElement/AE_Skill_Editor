using System;

namespace AE_Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UIElementAttribute : Attribute
    {
        public string prefabAssetName;//预制体名用于加载
        public int layerNum;//层级

        public UIElementAttribute(string prefabAssetName, int layerNum)
        {
            this.prefabAssetName = prefabAssetName;
            this.layerNum = layerNum;
        }
    }
}