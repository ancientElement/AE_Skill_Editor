using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AE_Framework
{
    /// <summary>
    /// 可用于面板
    /// 可序列化的字典
    /// </summary>
    [Serializable]
    public class Ex_Serialization_Dictionary_List<K, V>
    {
        [Serializable]
        public class ValueContainer
        {
            [SerializeField] public K Key;
            [SerializeField] public V Value;
        }

        [SerializeField, TableList]
        private List<ValueContainer> ValueList = new List<ValueContainer>();

        private List<K> KeyList = new List<K>();

        public List<K> Keys => KeyList;

        public List<ValueContainer> Values => ValueList;

        public V this[K key]
        {
            get
            {
                return GetValue(key);
            }
            set { this[key] = value; }
        }

        public bool ContainsKey(K key)
        {
            if (KeyList.Count == 0)
                OnValueChanged();
            return KeyList.Contains(key);
        }

        public V GetValue(K key)
        {
            if (KeyList.Count == 0)
                OnValueChanged();
            int index = KeyList.IndexOf(key);
            return ValueList[index].Value;
        }

        public bool TryGetValue(K key, ref V value)
        {
            if (KeyList.Count == 0)
                OnValueChanged();
            int index = KeyList.IndexOf(key);
            if (index != -1)
            {
                value = ValueList[index].Value;
                return true;
            }
            return false;
        }

        public void Remove(K key)
        {
            if (KeyList.Count == 0)
                OnValueChanged();
            int index = KeyList.IndexOf(key);
            RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            if (KeyList.Count == 0)
                OnValueChanged();
            KeyList.RemoveAt(index);
            ValueList.RemoveAt(index);
        }

        public void Add(K key = default, V value = default)
        {
            if (KeyList.Count == 0)
                OnValueChanged();
            KeyList.Add(key);
            ValueList.Add(new ValueContainer() { Key = key, Value = value });
        }

        public void Clear()
        {
            if (KeyList.Count == 0)
                OnValueChanged();
            KeyList.Clear();
            ValueList.Clear();
        }

        private void OnValueChanged()
        {
            KeyList.Clear();
            for (int i = 0; i < ValueList.Count; i++)
            {
                KeyList.Add(ValueList[i].Key);
            }
        }

#if UNITY_EDITOR

        [OnInspectorGUI]
        private void OnValueChangedEditor()
        {
            KeyList.Clear();
            for (int i = 0; i < ValueList.Count; i++)
            {
                KeyList.Add(ValueList[i].Key);
            }
        }

#endif
    }
}