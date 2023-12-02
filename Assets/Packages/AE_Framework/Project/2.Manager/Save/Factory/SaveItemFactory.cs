using System;
using System.Linq;
using UnityEngine;

namespace AE_Framework
{
    public static class SaveItemFactory
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="saveItemID"></param>
        /// <returns></returns>
        public static SaveItem CreateSaveItem(SaveMgrData saveMgrData, int saveItemID = -1)
        {
            if (saveItemID == -1)
            {
                saveItemID = GetSaveItemID(saveMgrData);
            }
            SaveItem item = new SaveItem(saveItemID, saveMgrData, DateTime.Now);
            saveMgrData.SaveItemList.Add(item);
            SaveMgrDataFactory.SaveSaveMgrData(saveMgrData);
            return item;
        }

        public static int GetSaveItemID(SaveMgrData saveMgrData)
        {
            int i = -1;
            SaveItem saveItem;
            do
            {
                i++;
                saveItem = saveMgrData.SaveItemList.Where(x => x.SaveID == i).FirstOrDefault();
            } while (saveItem != null);
            return i;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="saveItemID"></param>
        public static void DeleteSaveItem(SaveMgrData saveMgrData, int saveItemID)
        {
            SaveItem saveItem = saveMgrData.SaveItemList.Where(x => x.SaveID == saveItemID).FirstOrDefault();
            if (saveItem == null) return;
            if (saveItem.objects != null && saveItem.objects.Count > 0)
            {
                foreach (string item in saveItem.objects)
                {
                    SaveItemObjectFactory.DeleteObject(saveItem, item);
                }
            }
            saveMgrData.SaveItemList.Remove(saveItem);
            SaveMgrDataFactory.SaveSaveMgrData(saveMgrData);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="saveItemID"></param>
        /// <returns></returns>
        public static SaveItem GetSaveItem(SaveMgrData saveMgrData, int saveItemID)
        {
            SaveItem saveItem = saveMgrData.SaveItemList.Where(x => x.SaveID == saveItemID).FirstOrDefault();
            if (saveItem == null)
            {
                Debug.LogWarning($"saveItemID所属存档不存在{saveItemID}");
            }
            return saveItem;
        }
    }
}