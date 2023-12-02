using UnityEngine;

namespace AE_Framework
{
    public abstract class SingletonMonoMgr : MonoBehaviour
    {
        public abstract void Init();
    }

    public abstract class SingletonMonoMgr<T> : SingletonMonoMgr where T : SingletonMonoMgr<T>
    {
        public static T Instance;

        /// <summary>
        /// 管理器的初始化
        /// </summary>
        public override void Init()
        {
            Instance = this as T;
        }
    }
}