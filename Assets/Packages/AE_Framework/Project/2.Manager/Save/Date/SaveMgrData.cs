using System;
using System.Collections.Generic;

namespace AE_Framework
{
    /// <summary>
    /// 存档管理器的设置数据
    /// </summary>
    [Serializable]
    public class SaveMgrData
    {
        public string FileName;
        public string Path;

        // 当前的SaveItem
        public int currentSaveItemID = -1;

        // 所有存档的列表
        private List<SaveItem> saveItemList = new List<SaveItem>();

        public SaveMgrData(string path, string fileName)
        {
            Path = path;
            FileName = fileName;
        }

        public List<SaveItem> SaveItemList { get => saveItemList; }
    }
}