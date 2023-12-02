//using System;
//using UnityEditor;
//using System.Linq;
//using UnityEngine;
//using System.Reflection;
//using System.Collections.Generic;
//using UnityEditorInternal;
//using System.Collections;
//using System.Text;

//public class DrawDictionaryAttribute : Attribute
//{
//    public Type keyType;
//    public Type valueType;

//    public DrawDictionaryAttribute(Type keyType, Type valueType)
//    {
//        this.keyType = keyType;
//        this.valueType = valueType;
//    }
//}

//[CustomEditor(typeof(Test))]
//public class TestInspector : Editor
//{
//    public ReorderableList dictioary;

//    public override void OnInspectorGUI()
//    {
//        IEnumerable<FieldInfo> fields = GetAllFields(this.target, x => x.GetCustomAttributes(typeof(DrawDictionaryAttribute), true).Length > 0);
//        //Debug.Log(string.Join(" ,", fields));
//        //Rect rect = new Rect();
//        foreach (FieldInfo dic in fields)
//        {
//            //Dictionary<int, GameObject> dic_ = (Dictionary<int, GameObject>)dic.GetValue(this.target);
//            IDictionary dic_ = dic.GetValue(this.target) as IDictionary;
//            int count = dic_.Count;
//            IDictionaryEnumerator enu = dic_.GetEnumerator();


//            for (int i = 0; i < count; i++)
//            {
//                enu.MoveNext();
//                Debug.Log(enu.Key);
//                Debug.Log(enu.Value);
//            }
//        }
//    }



//    public static void ForeachClassProperties<T>(T model)
//    {
//        Type t = model.GetType();
//        PropertyInfo[] PropertyList = t.GetProperties();
//        foreach (PropertyInfo item in PropertyList)
//        {
//            string name = item.Name;
//            object value = item.GetValue(model, null);
//        }
//    }

//    public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
//    {
//        if (target == null)
//        {
//            Debug.LogError("The target object is null. Check for missing scripts.");
//            yield break;
//        }

//        List<Type> types = GetSelfAndBaseTypes(target);

//        for (int i = types.Count - 1; i >= 0; i--)
//        {
//            IEnumerable<FieldInfo> fieldInfos = types[i]
//                .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
//                .Where(predicate);

//            foreach (var fieldInfo in fieldInfos)
//            {
//                yield return fieldInfo;
//            }
//        }
//    }

//    public static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
//    {
//        if (target == null)
//        {
//            Debug.LogError("The target object is null. Check for missing scripts.");
//            yield break;
//        }

//        List<Type> types = GetSelfAndBaseTypes(target);

//        for (int i = types.Count - 1; i >= 0; i--)
//        {
//            IEnumerable<PropertyInfo> propertyInfos = types[i]
//                .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
//                .Where(predicate);

//            foreach (var propertyInfo in propertyInfos)
//            {
//                yield return propertyInfo;
//            }
//        }
//    }

//    private static List<Type> GetSelfAndBaseTypes(object target)
//    {
//        List<Type> types = new List<Type>()
//            {
//                target.GetType()
//            };

//        while (types.Last().BaseType != null)
//        {
//            types.Add(types.Last().BaseType);
//        }

//        return types;
//    }

//}
