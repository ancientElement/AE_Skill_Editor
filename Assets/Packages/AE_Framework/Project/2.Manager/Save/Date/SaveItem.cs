using System;
using System.Collections.Generic;

namespace AE_Framework
{
    /// <summary>
    /// 一个存档的数据
    /// </summary>
    [Serializable]
    public class SaveItem
    {
        private int m_saveID = -1;
        public int SaveID { get => m_saveID; }

        private SaveMgrData m_saveMgrData;
        public SaveMgrData SaveMgrData { get => m_saveMgrData; }

        public List<string> objects = new List<string>();

        public DateTime lastSaveTime;

        public string SaveItemPath => $"{m_saveMgrData.Path}{this.m_saveID}/";

        public SaveItem(int saveID, SaveMgrData m_saveMgrData, DateTime lastSaveTime)
        {
            this.m_saveID = saveID;
            this.lastSaveTime = lastSaveTime;
            this.m_saveMgrData = m_saveMgrData;
        }
    }
}