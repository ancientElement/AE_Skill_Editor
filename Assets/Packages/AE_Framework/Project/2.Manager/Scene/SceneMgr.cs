using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AE_Framework
{
    public static class SceneMgr
    {
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="name">场景名</param>
        /// <param name="fun">加载完场景需要做的事</param>
        public static void LoadScene(string name, LoadSceneMode loadSceneMode, Action fun = null)
        {
            //同步加载不能进行其他操作只能等待场景加载
            SceneManager.LoadScene(name, loadSceneMode);
            fun?.Invoke();
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun"></param>
        public static void LoadSceneAsync(string name, LoadSceneMode loadSceneMode, Action fun)
        {
            //异步加载可以进行其他操作比如加载
            //用MonoMgr开启协程
            MonoMgr.Instance.StartCoroutine(RealLoadScene(name, loadSceneMode, fun));
        }

        private static IEnumerator RealLoadScene(string name, LoadSceneMode loadSceneMode, Action fun)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(name, loadSceneMode);
            //当没加载完时做一些操作
            //如 触发 场景更新进度 事件, 将 场景更新进度 传到关心的地方
            //比如可以做进度条
            while (!ao.isDone)
            {
                //事件中心往外分发 场景更新进度
                EventCenter.TriggerEvent(EVENTNAME.LOAD_SCENE_ING, ao.progress);
                yield return ao.progress;
            }

            fun?.Invoke();
        }
    }
}