using UnityEngine;

namespace AE_Framework
{
    public static class SaveMgr
    {
        public static SaveMgrData saveMgrData { get; private set; }

        public static SaveMgrData settingsData { get; private set; }

        // 存档的保存
        private const string saveDirName = "saveData/";

        // 设置的保存：
        // 1.全局数据的保存（分辨率、按键设置）
        // 2.存档的设置保存。
        // 常规情况下，存档管理器自行维护
        private const string settingDirName = "setting/";

        // 存档文件夹路径
        public static string RootPath => Application.persistentDataPath + "/";

        static SaveMgr()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            saveMgrData = SaveMgrDataFactory.LoadSaveMgrData($"{RootPath}{saveDirName}", "saveData");
            settingsData = SaveMgrDataFactory.LoadSaveMgrData($"{RootPath}{settingDirName}", "settingData");
            if (saveMgrData.SaveItemList.Count == 0)
            {
                CreateSettingItem();
            }
        }

        #region Item

        public static SaveItem CreateSaveItem(int saveItemID = -1)
        {
            return SaveItemFactory.CreateSaveItem(saveMgrData, saveItemID);
        }

        public static void DeleteSaveItem(int saveItemID)
        {
            SaveItemFactory.DeleteSaveItem(saveMgrData, saveItemID);
        }

        public static SaveItem GetSaveItem(int saveItemID)
        {
            return SaveItemFactory.GetSaveItem(saveMgrData, saveItemID);
        }

        public static void SetCurrrentSaveItemID(int saveItemID)
        {
            SaveMgrDataFactory.SetCurrrentSaveItemID(saveMgrData, saveItemID);
        }

        #endregion Item

        #region object

        public static T LoadObj<T>(int saveItemID = -1) where T : class
        {
            if (saveItemID == -1)
            {
                saveItemID = saveMgrData.currentSaveItemID;
            }

            return SaveItemObjectFactory.LoadObj<T>(GetSaveItem(saveItemID));
        }

        public static void SaveObject<T>(object obj, int saveItemID = -1)
        {
            if (saveItemID == -1)
            {
                saveItemID = saveMgrData.currentSaveItemID;
            }

            SaveItemObjectFactory.SaveObject<T>(GetSaveItem(saveItemID), obj);
        }

        public static void DeleteObject<T>(int saveItemID = -1)
        {
            if (saveItemID == -1)
            {
                saveItemID = saveMgrData.currentSaveItemID;
            }

            SaveItemObjectFactory.DeleteObject<T>(GetSaveItem(saveItemID));
        }

        #endregion object

        #region Settings

        private static SaveItem CreateSettingItem()
        {
            return SaveItemFactory.CreateSaveItem(settingsData, 0);
        }

        public static SaveItem GetSettingItem()
        {
            return SaveItemFactory.GetSaveItem(settingsData, 0);
        }

        public static T LoadSettingObj<T>() where T : class
        {
            return SaveItemObjectFactory.LoadObj<T>(GetSettingItem());
        }

        public static void SaveSettingObject<T>(object obj)
        {
            SaveItemObjectFactory.SaveObject<T>(GetSettingItem(), obj);
        }

        public static void DeleteSettingObject<T>()
        {
            SaveItemObjectFactory.DeleteObject<T>(GetSettingItem());
        }

        #endregion Settings
    }
}