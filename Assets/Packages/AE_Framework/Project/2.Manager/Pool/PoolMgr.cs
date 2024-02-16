using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AE_Framework
{
    /// <summary>
    /// 缓存池(桌子)里的抽屉 游戏物体
    /// </summary>
    public class GameObjectPoolData
    {
        private Scene fatherScene;
        public List<GameObject> Datalist; //抽屉


        //当外部需要创建这个抽屉时
        //初始化抽屉
        //创建场景上的父节点(与物体同名) 挂载到场景上的pool
        //并放入第一个物体
        public GameObjectPoolData(Scene _poolScene, GameObject obj, string name)
        {
            fatherScene = _poolScene;
            Datalist = new List<GameObject>();
            //fatherObj = new GameObject(name);
            //fatherObj.transform.parent = poolObj.transform;
            PushObj(obj);
        }

        //拿到第一个抽屉里的物体
        //将物体 取消与场景上的父节点 的关联
        //激活物体
        //从抽屉里取出
        public GameObject GetObj()
        {
            GameObject obj = Datalist[0];
            //obj.transform.parent = null;
            obj.SetActive(true);
            Datalist.RemoveAt(0);
            return obj;
        }

        public GameObject GetObj(Vector3 position, Quaternion rotation)
        {
            GameObject obj = Datalist[0];
            //obj.transform.parent = null;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            Datalist.RemoveAt(0);
            return obj;
        }

        //将物体失活
        //设置父级为fatherObj
        //放进抽屉里
        public void PushObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.parent = null;
            SceneManager.MoveGameObjectToScene(obj, fatherScene);
            Datalist.Add(obj);
        }

        /// <summary>
        /// 删除自己
        /// </summary>
        public void Destory()
        {
            Datalist.Clear();
        }
    }

    /// <summary>
    /// 缓存池(桌子)里的抽屉 普通对象
    /// </summary>
    public class ObjectPoolData
    {
        public List<object> Datalist; //抽屉

        //当外部需要创建这个抽屉时
        //初始化抽屉
        //并放入第一个物体
        public ObjectPoolData(object obj)
        {
            Datalist = new List<object>();
            PushObj(obj);
        }

        //拿到第一个抽屉里的物体
        //从抽屉里取出
        public object GetObj()
        {
            object obj = Datalist[0];
            Datalist.RemoveAt(0);
            return obj;
        }

        //将物体失活
        //放进抽屉里
        public void PushObj(object obj)
        {
            Datalist.Add(obj);
        }
    }

    /// <summary>
    /// 缓存池(桌子)
    /// </summary>
    public static class PoolMgr
    {
        private static Scene poolScene;

        /// <summary>
        /// 游戏对象缓存池
        /// </summary>
        private static Dictionary<string, GameObjectPoolData> gameObjectPoolDic;

        /// <summary>
        /// 普通对象缓存池
        /// </summary>
        private static Dictionary<string, ObjectPoolData> objectPoolDic;

        static PoolMgr()
        {
            poolScene = SceneManager.CreateScene("PoolMgr");
            gameObjectPoolDic = new Dictionary<string, GameObjectPoolData>();
            objectPoolDic = new Dictionary<string, ObjectPoolData>();
        }

        #region 游戏对象缓存池

        public static GameObject GetGameObj(string name)
        {
            if (gameObjectPoolDic.ContainsKey(name) && gameObjectPoolDic[name].Datalist.Count > 0)
            {
                return gameObjectPoolDic[name].GetObj();
            }
            return null;
        }

        public static GameObject GetGameObj(string name, Vector3 position, Quaternion rotation)
        {
            if (gameObjectPoolDic.ContainsKey(name) && gameObjectPoolDic[name].Datalist.Count > 0)
            {
                return gameObjectPoolDic[name].GetObj(position, rotation);
            }
            return null;
        }

        /// <summary>
        /// 放进游戏对象缓存池
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public static void PushGameObj(string name, GameObject obj)
        {
            obj.SetActive(false);
            if (gameObjectPoolDic.ContainsKey(name))
            {
                gameObjectPoolDic[name].PushObj(obj);
            }
            else
            {
                gameObjectPoolDic.Add(name, new GameObjectPoolData(poolScene, obj, name));
            }
        }

        public static void PushGameObj(string name, GameObject obj, float time)
        {
            MonoMgr.Instance.StartCoroutine(PushInPool(name, obj, time));
        }

        private static IEnumerator PushInPool(string name, GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            PushGameObj(name, obj);
            yield break;
        }

        #endregion 游戏对象缓存池

        #region 普通对象缓存池

        /// <summary>
        /// 从对象池拿到
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetObj<T>() where T : class, new()
        {
            string name = typeof(T).FullName;
            if (objectPoolDic.ContainsKey(name) && objectPoolDic[name].Datalist.Count > 0)
            {
                return objectPoolDic[name].GetObj() as T;
            }
            else
            {
                return new T();
            }
        }

        /// <summary>
        /// 放进对象池
        /// </summary>
        /// <param name="obj"></param>
        public static void PushObj<T>(object obj)
        {
            string name = typeof(T).FullName;
            if (objectPoolDic.ContainsKey(name))
            {
                objectPoolDic[name].PushObj(obj);
            }
            else
            {
                objectPoolDic.Add(name, new ObjectPoolData(obj));
            }
        }

        #endregion 普通对象缓存池

        #region 清理对象池

        public static void ClearAllGameObject()
        {
            Clear(true, false);
        }

        public static void ClearAllObject()
        {
            Clear(false, true);
        }

        public static void Clear(bool isGameObject = true, bool isObject = true)
        {
            if (isGameObject)
            {
                if (gameObjectPoolDic == null || gameObjectPoolDic.Count == 0) return;

                foreach (var item in gameObjectPoolDic)
                {
                    item.Value.Destory();
                }

                gameObjectPoolDic.Clear();
            }

            if (isObject)
            {
                if (objectPoolDic == null || objectPoolDic.Count == 0) return;

                objectPoolDic.Clear();
            }
        }

        public static void ClearGameObject(string name)
        {
            if (!gameObjectPoolDic.ContainsKey(name)) return;

            gameObjectPoolDic[name].Destory();
            gameObjectPoolDic.Remove(name);
        }

        public static void ClearObject<T>()
        {
            if (!objectPoolDic.ContainsKey(typeof(T).FullName)) return;
            objectPoolDic.Remove(typeof(T).FullName);
        }

        #endregion 清理对象池
    }
}