using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace AE_Framework
{
    /// <summary>
    /// 不可用于面板只能运行时用
    /// 可序列化的字典
    /// </summary>
    [Serializable]
    public class Serialization_Dictionary<K, V> : Dictionary<K, V>
    {
        [SerializeField] public List<K> KeyList;
        [SerializeField] public List<V> ValueList;

        //序列化的时候
        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
            Debug.Log(streamingContext);
            KeyList = new List<K>(this.Count);
            ValueList = new List<V>(this.Count);
            foreach (var item in this)
            {
                KeyList.Add(item.Key);
                ValueList.Add(item.Value);
            }
        }

        [OnDeserialized]//反序列化的时候
        private void Deserialized(StreamingContext streamingContext)
        {
            this.Clear();
            for (int i = 0; i < KeyList.Count; i++)
            {
                this.Add(KeyList[i], ValueList[i]);
            }
            KeyList.Clear();
            ValueList.Clear();
        }
    }
}