using Codice.Client.BaseCommands.BranchExplorer;
using Excel;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace AE_Framework
{
    public static class ExcelTool
    {
        /// <summary>
        /// Excel文件存放位置
        /// </summary>
        public static string EXCEL_PATH = Application.dataPath + "/Res/Excel/";
        /// <summary>
        /// 数据结构类存放位置
        /// </summary>
        public static string DATA_ClASS_PATH = Application.dataPath + "/Scripts/ExcelData/DataClass/";
        /// <summary>
        /// 容器类存放位置
        /// </summary>
        public static string DATA_CONTAINER_PATH = Application.dataPath + "/Scripts/ExcelData/Container/";
        /// <summary>
        /// JSON存放位置
        /// </summary>
        public static string JSON_CONTAINER_PATH = Application.dataPath + "/Scripts/ExcelData/JSON/";
        /// <summary>
        /// Excel表开始位置
        /// </summary>
        public static int BEGING_INDEX = 4;
        /// <summary>
        /// SO的位置
        /// </summary>
        public static readonly string SO_Data_Path = "Assets/Scripts/ExcelData/SO/";


        /// <summary>
        /// 生成excel对应数据结构类、容器类
        /// </summary>
        public static void GenarateExcel()
        {
            //得到指定文件夹xlsx文件
            DirectoryInfo dInfo = Directory.CreateDirectory(EXCEL_PATH);
            //得到xlsx文件
            FileInfo[] fInfo = dInfo.GetFiles();
            foreach (var item in fInfo)
            {
                GenarateExcel(item);
            }
        }
        public static void GenarateExcel(FileInfo fInfo)
        {
            //数据表容器
            DataTableCollection tableCollection;

            //判断xlsx和xls文件
            if (fInfo.Extension != ".xlsx" && fInfo.Extension != ".xls")
                return;

            //得到文件所有的表
            using (FileStream fs = fInfo.Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = excelDataReader.AsDataSet().Tables;
                fs.Close();
            }

            //遍历表的信息
            foreach (DataTable table in tableCollection)
            {
                Debug.Log("已加载表:" + table.TableName);
                //生成数据结构类
                GenerateExcelDataClass(table);
                //生成容器类
                GenerateExcelContainerClass(table);
            }
        }


        /// <summary>
        /// 生成二进制
        /// </summary>
        /// <param name="table"></param>
        public static void GenerateExcelBinary(DataTable table)
        {
            if (!Directory.Exists(Excel2ObjMgr.DATA_BINARY_PATH))
                Directory.CreateDirectory(Excel2ObjMgr.DATA_BINARY_PATH);
            using (FileStream fs = new FileStream(Excel2ObjMgr.DATA_BINARY_PATH + table.TableName + Excel2ObjMgr.BINARY_EXTANTION, FileMode.OpenOrCreate, FileAccess.Write))
            {
                //总行数
                fs.Write(BitConverter.GetBytes(table.Rows.Count - BEGING_INDEX), 0, sizeof(int));
                //主键变量名
                string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(keyName);
                //存string长度
                fs.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                //存string
                fs.Write(bytes, 0, bytes.Length);
                DataRow row;
                DataRow variableTypeName = GetVariableTypeNameRow(table);
                for (int i = BEGING_INDEX; i < table.Rows.Count; i++)
                {
                    row = table.Rows[i];
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        switch (variableTypeName[j].ToString())
                        {
                            case "int":
                                fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, sizeof(int));
                                break;
                            case "float":
                                fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, sizeof(float));
                                break;
                            case "bool":
                                fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, sizeof(bool));
                                break;
                            case "string":
                                bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                                fs.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                                fs.Write(bytes, 0, bytes.Length);
                                break;
                        }
                    }
                }
                fs.Close();
            }
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 生成SO
        /// </summary>
        /// <param name="table"></param>
        public static void GenerateExcelSO(FileInfo fInfo)
        {
            //数据表容器
            DataTableCollection tableCollection;

            //判断xlsx和xls文件
            if (fInfo.Extension != ".xlsx" && fInfo.Extension != ".xls")
                return;

            //得到文件所有的表
            using (FileStream fs = fInfo.Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = excelDataReader.AsDataSet().Tables;
                fs.Close();
            }

            //遍历表的信息
            foreach (DataTable table in tableCollection)
            {
                GenerateExcelSO(table, $"{table.TableName}Container", table.TableName);
            }
        }
        private static void GenerateExcelSO(DataTable table, string containerType, string classType)
        {
            Type _classType = GetType(classType);
            Type _containerType = GetType(containerType);
            if (_classType == null || _containerType == null)
            {
                if (_classType == null)
                {
                    Debug.LogWarning($"不存在该结构类{classType}");
                }
                else
                {
                    Debug.LogWarning($"不存在该容器类{containerType}");
                }
                return;
            }
            GenerateExcelSO(table, _containerType, _classType);
        }
        private static void GenerateExcelSO(DataTable table, Type containerType, Type classType)
        {
            if (!Directory.Exists(SO_Data_Path))
                Directory.CreateDirectory(SO_Data_Path);

            //主键变量名
            string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
            Debug.Log(keyName);

            ScriptableObject container = ScriptableObject.CreateInstance(containerType);
            //得到字段
            FieldInfo[] infos = classType.GetFields();

            DataRow row;
            DataRow variableTypeName = GetVariableTypeNameRow(table);
            DataRow variableName = GetVariableNameRow(table);

            //遍历所有行
            for (int i = BEGING_INDEX; i < table.Rows.Count; i++)
            {
                //实例化数据结构类
                object dataObj = Activator.CreateInstance(classType);
                //每一行
                row = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    string tempVariableName = (string)variableName[j];//变量名
                    FieldInfo tempFileInfo = infos.Where(x => x.Name == tempVariableName).FirstOrDefault();//字段

                    switch (variableTypeName[j].ToString())
                    {
                        case "int":
                            tempFileInfo.SetValue(dataObj, int.Parse(row[j].ToString()));
                            break;
                        case "float":
                            tempFileInfo.SetValue(dataObj, float.Parse(row[j].ToString()));
                            break;
                        case "bool":
                            tempFileInfo.SetValue(dataObj, bool.Parse(row[j].ToString()));
                            break;
                        case "string":
                            tempFileInfo.SetValue(dataObj, row[j].ToString());
                            break;
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

            AssetDatabase.CreateAsset(container, SO_Data_Path + "m_" + table.TableName + ".asset");//在传入的路径中创建资源
            EditorUtility.SetDirty(container);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 生成容器类SO
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelContainerClass(DataTable table)
        {
            //主键
            int keyIndex = GetKeyIndex(table);
            //类型
            DataRow variableTypeName = GetVariableTypeNameRow(table);
            //文件夹
            if (!Directory.Exists(DATA_CONTAINER_PATH))
                Directory.CreateDirectory(DATA_CONTAINER_PATH);
            //拼接脚本
            string scriptString = "using UnityEngine;\nusing Sirenix.OdinInspector;\nusing System.Collections.Generic;\n";
            scriptString += $"public class {table.TableName}Container : SerializedScriptableObject" + "\n{\n";
            scriptString += "    public  Dictionary<" + variableTypeName[keyIndex] + "," + table.TableName + ">";
            scriptString += "dataDic = new " + "Dictionary<" + variableTypeName[keyIndex] + "," + table.TableName + ">();\n";
            scriptString += "}";
            File.WriteAllText(DATA_CONTAINER_PATH + table.TableName + "Container.cs", scriptString);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成数据结构类
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelDataClass(DataTable table)
        {
            //字段名
            DataRow variableName = GetVariableNameRow(table);
            //字段类型
            DataRow variableTypeName = GetVariableTypeNameRow(table);
            //判断文件夹
            if (!Directory.Exists(DATA_ClASS_PATH))
                Directory.CreateDirectory(DATA_ClASS_PATH);
            //拼接脚本
            string scriptString = "public class " + table.TableName + "\n{\n";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                scriptString += "    public " + variableTypeName[i].ToString() + " " + variableName[i].ToString() + ";\n";
            }
            scriptString += "}";
            //保存脚本
            File.WriteAllText(DATA_ClASS_PATH + table.TableName + ".cs", scriptString);
            //刷新
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成JSON
        /// </summary>
        public static void GenerateJson(FileInfo fInfo)
        {
            //数据表容器
            DataTableCollection tableCollection;

            //判断xlsx和xls文件
            if (fInfo.Extension != ".xlsx" && fInfo.Extension != ".xls")
                return;

            //得到文件所有的表
            using (FileStream fs = fInfo.Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = excelDataReader.AsDataSet().Tables;
                fs.Close();
            }

            //遍历表的信息
            foreach (DataTable table in tableCollection)
            {
                GenerateExcelJson(table);
            }
        }
        private static void GenerateExcelJson(DataTable table)
        {
            if (!Directory.Exists(JSON_CONTAINER_PATH))
                Directory.CreateDirectory(JSON_CONTAINER_PATH);

            //主键变量名
            string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
            // Debug.Log(keyName);

            string jsontxt = string.Empty;

            DataRow row;
            DataRow variableTypeName = GetVariableTypeNameRow(table);
            DataRow variableName = GetVariableNameRow(table);

            jsontxt += "[\r\n";

            //遍历所有行
            for (int i = BEGING_INDEX; i < table.Rows.Count; i++)
            {
                //每一行
                row = table.Rows[i];

                jsontxt += "{\r\n";

                for (int j = 0; j < table.Columns.Count; j++)
                {
                    string tempVariableName = (string)variableName[j];//变量名

                    switch (variableTypeName[j].ToString())
                    {
                        case "int":
                            jsontxt += $"\"{tempVariableName}\":{int.Parse(row[j].ToString())}";
                            break;
                        case "float":
                            jsontxt += $"\"{tempVariableName}\":{float.Parse(row[j].ToString())}";
                            break;
                        case "bool":
                            jsontxt += $"\"{tempVariableName}\":{bool.Parse(row[j].ToString())})";
                            break;
                        case "string":
                            jsontxt += $"\"{tempVariableName}\":\"{row[j]}\"";
                            break;
                    }

                    if (j != table.Columns.Count - 1)
                        jsontxt += ",";
                    jsontxt += "\r\n";
                }

                jsontxt += "}";
                if (i != table.Rows.Count - 1)
                    jsontxt += ",";
                jsontxt += "\r\n";
            }

            jsontxt += "]";

            File.WriteAllText(JSON_CONTAINER_PATH + $"{table.TableName}.json", jsontxt);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 变量名行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataRow GetVariableNameRow(DataTable table)
        {
            return table.Rows[0];
        }

        /// <summary>
        /// 变量名类型行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataRow GetVariableTypeNameRow(DataTable table)
        {
            return table.Rows[1];
        }

        /// <summary>
        /// 获取主键index
        /// </summary>
        /// <returns></returns>
        private static int GetKeyIndex(DataTable table)
        {
            DataRow keyRow = table.Rows[2];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (keyRow[i].ToString() == "key")
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// 获取类型全局
        /// </summary>
        /// <param name="sctriptName"></param>
        /// <returns></returns>
        private static Type GetType(string sctriptName)
        {
            if (sctriptName == null) return null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var item in assemblies)
            {
                if (item.FullName.StartsWith("UnityEngin") || item.FullName.StartsWith("UnityEditor") || item.FullName.StartsWith("System") || item.FullName.StartsWith("Microsoft"))
                { continue; }

                Type t = item.GetTypes().Where(x => x.FullName == sctriptName).FirstOrDefault();
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }
    }
}