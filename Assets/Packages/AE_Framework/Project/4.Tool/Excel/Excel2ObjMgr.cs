using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AE_Framework
{
    public class Excel2ObjMgr : BaseManager<Excel2ObjMgr>
    {
        /// <summary>
        /// 二进制文件后缀名
        /// </summary>
        public static string BINARY_EXTANTION = ".AE";

        /// <summary>
        /// <summary>
        ///  二进制excel数据文件夹
        /// </summary>
        public static string DATA_BINARY_PATH = Application.streamingAssetsPath + "/Excel/Binary/";

        public void InitData()
        {
        }

        /// <summary>
        /// 存储excel表数据容器
        /// </summary>
        public Dictionary<string, object> tableDic = new Dictionary<string, object>();

        /// <summary>
        /// 加载excel到内存中
        /// </summary>
        /// <typeparam name="T">容器类对象</typeparam>
        /// <typeparam name="K">数据结构类</typeparam>
        public void LoadTable<T, K>()
        {
            LoadTable(typeof(T), typeof(K));
        }

        public void LoadTable(Type containerType, Type classType)
        {
            using (FileStream fs = File.Open(DATA_BINARY_PATH + containerType.Name + BINARY_EXTANTION, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                //记录索引
                int index = 0;
                //读取行数据
                int count = BitConverter.ToInt32(bytes, index);
                index += sizeof(int);
                //读取主键名
                int keyNameLength = BitConverter.ToInt32(bytes, index);
                index += sizeof(int);
                string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
                index += keyNameLength;
                object container = Activator.CreateInstance(containerType);
                //得到字段
                FieldInfo[] infos = classType.GetFields();
                //读取每一行
                for (int i = 0; i < count; i++)
                {
                    //实例化数据结构类
                    object dataObj = Activator.CreateInstance(classType);
                    //遍历字段 设置值
                    foreach (var field in infos)
                    {
                        if (field.FieldType == typeof(int))
                        {
                            field.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                            index += sizeof(int);
                        }
                        else if (field.FieldType == typeof(string))
                        {
                            keyNameLength = BitConverter.ToInt32(bytes, index);
                            index += sizeof(int);
                            field.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, keyNameLength));
                            index += keyNameLength;
                        }
                        else if (field.FieldType == typeof(bool))
                        {
                            field.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                            index += sizeof(bool);
                        }
                        else if (field.FieldType == typeof(float))
                        {
                            field.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                            index += sizeof(float);
                        }
                    }
                    //添加数据到容器变量中
                    object dicObject = containerType.GetField("dataDic").GetValue(container);
                    //Add方法
                    MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                    //主键值
                    object keyValue = classType.GetField(keyName).GetValue(dataObj);
                    //添加每一行进容器类
                    mInfo.Invoke(dicObject, new object[] { keyValue, dataObj });
                }
                //把表记录下来
                tableDic.Add(containerType.Name, container);
            }
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <typeparam name="T">表容器类型</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T : class
        {
            string tableName = typeof(T).Name;
            if (tableDic.ContainsKey(tableName))
                return tableDic[tableName] as T;
            return null;
        }
    }
}