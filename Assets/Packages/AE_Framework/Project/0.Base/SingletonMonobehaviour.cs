//using Sirenix.OdinInspector;
using UnityEngine;

namespace AE_Framework
{
    /// <summary>
    /// 基础mono的单例基类
    /// 只能手动在场景上添加一个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        //返回单例
        public static T Instance
        { get { return instance; } protected set { instance = value; } }

        //虚方法让子类重写
        protected virtual void Awake()
        {
            //手动加这个脚本到对象上才行
            instance = this as T;
        }
    }
}