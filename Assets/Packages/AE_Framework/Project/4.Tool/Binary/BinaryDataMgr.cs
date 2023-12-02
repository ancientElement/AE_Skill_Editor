using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AE_Framework
{
    public class BinaryDataMgr : BaseManager<BinaryDataMgr>
    {
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="savePath"></param>
        /// <param name="binaryExtantion"></param>
        /// <param name="fileName"></param>
        public void Save(object obj, string savePath, string fileName, string binaryExtantion)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            using (FileStream fs = new FileStream(savePath + fileName + binaryExtantion, FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
                fs.Close();
            }
        }

        /// <summary>
        /// 加载对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="savePath"></param>
        /// <param name="binaryExtantion"></param>
        /// <returns></returns>
        public T Load<T>(string savePath, string fileName, string binaryExtantion) where T : class
        {
            if (!File.Exists(savePath + fileName))
                return default(T);
            T obj;
            using (FileStream fs = File.Open(savePath + fileName + binaryExtantion, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                obj = (T)bf.Deserialize(fs);
            }
            return obj;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filePath"></param>
        public void Delete(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"不存在此路径{filePath}");
                return;
            }
            File.Delete(filePath);
        }
    }
}