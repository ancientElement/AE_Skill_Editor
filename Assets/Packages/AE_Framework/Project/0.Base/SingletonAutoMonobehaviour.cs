using UnityEngine;

namespace AE_Framework
{
    /// <summary>
    /// 自动生成的继承mono的单例
    /// 不会销毁的对象
    /// </summary>
    public abstract class SingletonAutoMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    //创建依附的对象
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    DontDestroyOnLoad(obj);//场景转换不删除
                    instance = obj.AddComponent<T>();
                }
                return instance;
            }
            protected set { instance = value; }
        }
    }
}