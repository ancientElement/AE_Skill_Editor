using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AE_Framework
{
    /// <summary>
    /// 资源加载管理器
    /// 自动实例化GameObject
    /// </summary>
    public static class ResMgr
    {
        #region Resources

        /// <summary>
        /// Resources 同步资源加载
        /// </summary>
        public static T ResourcesLoad<T>(string assetName, Transform parent = null) where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(assetName);
            //判断资源是否是GameObject对象
            //是则实例化后方返回
            if (res is GameObject)
                return GameObject.Instantiate(res, parent);
            else
                return res;
        }

        //这里我们要约束 泛型Ｔ　为UnityEngine.Object
        //因为 Resources 加载的是 UnityEngine.Object
        //使用回调函数传递参数
        //因为在协程函数里不能直接return回去
        /// <summary>
        /// Resources异步加载资源
        /// </summary>
        public static void ResourcesLoadAsync<T>(string assetName, Action<T> callback, Transform parent = null)
            where T : UnityEngine.Object
        {
            MonoMgr.Instance.StartCoroutine(ResourcesLoadAsyncIEnumerator<T>(assetName, callback, parent));
        }

        private static IEnumerator ResourcesLoadAsyncIEnumerator<T>(string name, Action<T> callback,
            Transform parent = null)
            where T : UnityEngine.Object
        {
            ResourceRequest res = Resources.LoadAsync<T>(name);

            //等待加载完毕
            yield return res;

            //加载完毕后判断资源是否是GameObject对象
            //是则实例化后方返回
            if (res.asset is GameObject)
                callback(GameObject.Instantiate(res.asset, parent) as T);
            else
                callback(res.asset as T);
        }

        /// <summary>
        /// Resources异步加载资源
        /// </summary>
        public static async UniTask<T> ResourcesLoadAsync<T>(string assetName, Transform parent = null)
            where T : UnityEngine.Object
        {
            ResourceRequest res = Resources.LoadAsync<T>(assetName);
            //等待加载完毕
            await res;
            //加载完毕后判断资源是否是GameObject对象
            //是则实例化后方返回
            if (res.asset is GameObject)
                return GameObject.Instantiate(res.asset, parent) as T;
            else
                return res.asset as T;
        }

        #endregion Resources

        #region Addressable

        /// <summary>
        /// Addressable同步加载游戏物体
        /// </summary>
        public static T AddressableLoad<T>(string assetName, Transform parent = null) where T : UnityEngine.Object
        {
            if (typeof(T) == typeof(GameObject))
            {
                return Addressables.InstantiateAsync(assetName).WaitForCompletion() as T;
            }
            else
            {
                return Addressables.LoadAssetAsync<T>(assetName).WaitForCompletion();
            }
        }

        /// <summary>
        /// Addressable同步加载游戏物体
        /// </summary>
        public static T AddressableLoad<T>(AssetReference asset) where T : UnityEngine.Object
        {
            if (typeof(T) == typeof(GameObject))
            {
                return Addressables.InstantiateAsync(asset).WaitForCompletion() as T;
            }
            else
            {
                return Addressables.LoadAssetAsync<T>(asset).WaitForCompletion();
            }
        }

        /// <summary>
        ///  Addressable异步加载游戏物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="callBack"></param>
        /// <param name="parent"></param>
        public static void AddressableLoadAsync<T>(string assetName, Action<T> callBack = null, Transform parent = null)
            where T : class
        {
            MonoMgr.Instance.StartCoroutine(AddressableDoLoadAsync<T>(assetName, callBack, parent));
        }

        private static IEnumerator AddressableDoLoadAsync<T>(string assetName, Action<T> callBack = null,
            Transform parent = null)
            where T : class
        {
            // 不通过缓存池
            if (typeof(T) == typeof(GameObject))
            {
                var request = Addressables.InstantiateAsync(assetName, parent);
                yield return request;
                callBack?.Invoke(request.Result as T);
            }
            else
            {
                var request = Addressables.LoadAssetAsync<T>(assetName);
                yield return request;
                callBack?.Invoke(request.Result);
            }
        }

        /// <summary>
        /// Addressable异步加载游戏物体
        /// </summary>
        /// <typeparam name="T">具体的组件</typeparam>
        public static void AddressableLoadAsync<T>(AssetReference assetReference, Action<T> callBack = null,
            Transform parent = null) where T : class
        {
            MonoMgr.Instance.StartCoroutine(AddressableDoLoadGameObjectAsync<T>(assetReference, callBack, parent));
        }

        private static IEnumerator AddressableDoLoadGameObjectAsync<T>(AssetReference assetReference,
            Action<T> callBack = null,
            Transform parent = null) where T : class
        {
            if (typeof(T) == typeof(GameObject))
            {
                var request = Addressables.InstantiateAsync(assetReference, parent);
                yield return request;
                callBack?.Invoke(request.Result as T);
            }
            else
            {
                var request = Addressables.LoadAssetAsync<T>(assetReference);
                yield return request;
                callBack?.Invoke(request.Result);
            }
        }

        /// <summary>
        /// UniTask Addressable异步加载游戏物体
        /// </summary>
        public static async UniTask<T> AddressableLoadUniTaskAsync<T>(string assetName,
            Action<GameObject> callBack = null,
            Transform parent = null) where T : class
        {
            // 不通过缓存池
            if (typeof(T) == typeof(GameObject))
            {
                var handel = Addressables.InstantiateAsync(assetName);
                await handel;
                return handel.Result as T;
            }
            else
            {
                var handel = Addressables.LoadAssetAsync<T>(assetName);
                await handel;
                return handel.Result as T;
            }
        }

        /// <summary>
        /// UniTask Addressable异步加载游戏物体
        /// </summary>
        public static async UniTask<T> AddressableLoadUniTaskAsync<T>(AssetReference assetReference,
            Action<GameObject> callBack = null, Transform parent = null) where T : class
        {
            // 不通过缓存池
            if (typeof(T) == typeof(GameObject))
            {
                var handel = Addressables.InstantiateAsync(assetReference);
                await handel;
                return handel.Result as T;
            }
            else
            {
                var handel = Addressables.LoadAssetAsync<T>(assetReference);
                await handel;
                return handel.Result as T;
            }
        }

        #endregion Addressable

        /// <summary>
        /// 释放Addressables资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void Release<T>(T obj)
        {
            Addressables.Release(obj);
        }

        /// <summary>
        /// 释放实例
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool ReleaseInstance(GameObject gameObject)
        {
            return Addressables.ReleaseInstance(gameObject);
        }
    }
}