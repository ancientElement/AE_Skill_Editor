using System;
using UnityEngine;

namespace AE_Framework
{
    public static class SaveItemObjectFactory
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void SaveObject<T>(SaveItem saveItem, object obj)
        {
            if (!saveItem.objects.Contains(typeof(T).FullName))
            {
                saveItem.objects.Add(typeof(T).FullName);
            }

            BinaryDataMgr.Instance.Save(obj, saveItem.SaveItemPath, typeof(T).FullName, string.Empty);
            saveItem.lastSaveTime = DateTime.Now;
            SaveMgrDataFactory.SaveSaveMgrData(saveItem.SaveMgrData);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DeleteObject<T>(SaveItem saveItem)
        {
            if (saveItem.objects == null || saveItem.objects.Count == 0) return;

            if (!saveItem.objects.Contains(typeof(T).FullName)) return;

            DeleteObject(saveItem, $"{typeof(T).FullName}");
            saveItem.objects.Remove(typeof(T).FullName);

            SaveMgrDataFactory.SaveSaveMgrData(saveItem.SaveMgrData);
        }

        public static void DeleteObject(SaveItem saveItem, string objectTypeFullName)
        {
            BinaryDataMgr.Instance.Delete($"{saveItem.SaveItemPath}/{objectTypeFullName}");
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadObj<T>(SaveItem saveItem) where T : class
        {
            if (!saveItem.objects.Contains(typeof(T).FullName))
            {
                Debug.LogWarning($"未包含该数据{typeof(T)}");
                return null;
            }
            T temp = BinaryDataMgr.Instance.Load<T>(saveItem.SaveItemPath, $"{typeof(T).FullName}", string.Empty);
            return temp;
        }
    }
}