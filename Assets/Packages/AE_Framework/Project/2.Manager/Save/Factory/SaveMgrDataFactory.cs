using UnityEngine;

namespace AE_Framework
{
    public static class SaveMgrDataFactory
    {
        /// <summary>
        /// 加载存档管理器数据
        /// </summary>
        public static SaveMgrData LoadSaveMgrData(string path, string fileName)
        {
            SaveMgrData temp = BinaryDataMgr.Instance.Load<SaveMgrData>(path, fileName, string.Empty);
            if (temp != null && temp != default(SaveMgrData))
            {
                return temp;
            }
            else
            {
                return new(path, fileName);
            }
        }

        /// <summary>
        /// 设置当前SaveItem
        /// </summary>
        /// <param name="saveMgrData"></param>
        /// <param name="saveItemID"></param>
        public static void SetCurrrentSaveItemID(SaveMgrData saveMgrData, int saveItemID)
        {
            saveMgrData.currentSaveItemID = saveItemID;
            SaveSaveMgrData(saveMgrData);
        }

        /// <summary>
        /// 保存存档管理器
        /// </summary>
        public static void SaveSaveMgrData(SaveMgrData saveMgrData)
        {
            if (saveMgrData == null)
            {
                Debug.LogWarning("没有存档管理器数据");
                return;
            }
            BinaryDataMgr.Instance.Save(saveMgrData, saveMgrData.Path, saveMgrData.FileName, string.Empty);
        }
    }
}