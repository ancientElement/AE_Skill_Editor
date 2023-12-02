using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AE_Framework
{
    public class MusicMgr : SingletonMonoMgr<MusicMgr>
    {
        [SerializeField] private GameObject Music_base; //音效挂载的游戏对象

        private AudioSource bkMusic; //背景音乐组件
        [SerializeField] private float bkVolume = 0.5f; //背景音乐音量

        private List<AudioSource> soundMusicList = new List<AudioSource>(); //音效组件

        //Resources下的加载文件夹
        private static readonly string AudioSourceResourcesDir = "AudioSource";

        /// <summary>
        /// 改变背景音乐音量
        /// </summary>
        /// <param name="volume"></param>
        public void ChageBKVolume(float volume)
        {
            bkVolume = volume;
            if (bkMusic == null) return;
            bkMusic.volume = bkVolume;
        }

        /// <summary>
        /// 设置AudioSource配置
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="audioClip"></param>
        /// <param name="isLoop"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="mute"></param>
        private void SetAudioSource(AudioSource audioSource, AudioClip audioClip, bool isLoop = false,
            float spatialBlend = 0, bool mute = false, float volumn = 0.5f)
        {
            audioSource.clip = audioClip;
            //设置音量
            audioSource.volume = Mathf.Clamp01(volumn);
            //设置音乐片段是否循环
            audioSource.loop = isLoop;
            //设置3D效果
            audioSource.spatialBlend = Mathf.Clamp01(spatialBlend);
            //设置静音
            audioSource.mute = mute;
        }

        #region 背景音乐

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayBKMusic(string name)
        {
            //播放背景音乐
            //bkMusic为空则为bkMusic创建挂载的空对象
            //异步加载后播放
            if (bkMusic == null)
            {
                bkMusic = Music_base.AddComponent<AudioSource>();
            }

            SetAudioSource(bkMusic, ResMgr.AddressableLoad<AudioClip>(name), isLoop: true);
            bkMusic.Play();
        }

        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        public void PauseBKMusic()
        {
            //判断是否有背景音乐组件
            if (bkMusic == null) return;
            bkMusic.Pause();
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBkMusic()
        {
            if (bkMusic == null) return;
            bkMusic.Stop();
            //释放资源
            ResMgr.Release(bkMusic.clip);
            bkMusic.clip = null;
        }

        #endregion 背景音乐

        #region 音效

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="audioSource"></param>
        public void StopSoundMusic(AudioSource audioSource)
        {
            if (soundMusicList.Contains(audioSource))
            {
                GameObject.Destroy(audioSource);
                //Debug.Log(audioSource);
                soundMusicList.Remove(audioSource);
                //Debug.Log(audioSource);
                audioSource.Stop();
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isLoop"></param>
        /// <param name="callback"></param>
        public void PlaySoundMusic(string name, bool isLoop = false, Action<AudioSource> callback = null,
            float spatialBlend = 0, bool mute = false, float volumn = 0.5f)
        {
            //音效加载完后
            ODPlaySoundMusic(ResMgr.AddressableLoad<AudioClip>(name), callback, isLoop, spatialBlend, mute,
                volumn);
        }

        /// <summary>
        /// 播放音效 3D
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isLoop"></param>
        /// <param name="callback"></param>
        public void PlaySoundMusic(string name, Vector3 worldPosition, Action<AudioSource> callback = null,
            bool isLoop = false, float spatialBlend = 0, bool mute = false, float volumn = 0.5f)
        {
            //音效加载完后
            ODPlaySoundMusic(ResMgr.AddressableLoad<AudioClip>(name), worldPosition, callback, isLoop,
                spatialBlend,
                mute);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySoundMusic(AudioClip audioClip, bool isLoop = false, Action<AudioSource> callback = null,
            float spatialBlend = 0, bool mute = false, float volumn = 0.5f)
        {
            ODPlaySoundMusic(audioClip, callback, isLoop, spatialBlend, mute, volumn);
        }

        /// <summary>
        /// 播放音效 3D
        /// </summary>
        public void PlaySoundMusic(AudioClip audioClip, Vector3 worldPosition, Action<AudioSource> callback = null,
            bool isLoop = false, float spatialBlend = 0, bool mute = false, float volumn = 0.5f)
        {
            //音效加载完后
            ODPlaySoundMusic(audioClip, worldPosition, callback, isLoop, spatialBlend, mute, volumn);
        }

        /// <summary>
        /// 播放音效逻辑
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="worldPosition"></param>
        /// <param name="isLoop"></param>
        /// <param name="callback"></param>
        private void ODPlaySoundMusic(AudioClip audioClip, Action<AudioSource> callback = null,
            bool isLoop = false, float spatialBlend = 0, bool mute = false, float volumn = 0.5f)
        {
            GameObject obj = PoolMgr.GetGameObj("AudioSource");
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            //添加进soundMusicList
            soundMusicList.Add(audioSource);
            //设置音效
            SetAudioSource(audioSource, audioClip, isLoop, spatialBlend, mute, volumn);
            audioSource.Play();
            //协程回收AudioSource
            StartCoroutine(RecycleAudioSource(audioClip, obj, audioSource));
        }

        /// <summary>
        /// 播放音效逻辑 3D
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="worldPosition"></param>
        /// <param name="isLoop"></param>
        /// <param name="callback"></param>
        private void ODPlaySoundMusic(AudioClip audioClip, Vector3 worldPosition,
            Action<AudioSource> callback = null, bool isLoop = false, float spatialBlend = 0, bool mute = false,
            float volumn = 0.5f)
        {
            GameObject obj = PoolMgr.GetGameObj(AudioSourceResourcesDir, worldPosition, Quaternion.identity);
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            //添加进soundMusicList
            soundMusicList.Add(audioSource);
            //设置音效
            SetAudioSource(audioSource, audioClip, isLoop, spatialBlend, mute, volumn);
            audioSource.Play();
            //协程回收AudioSource
            StartCoroutine(RecycleAudioSource(audioClip, obj, audioSource));
        }

        /// <summary>
        /// 回收音效
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="obj"></param>
        /// <param name="audioSource"></param>
        /// <param name="callback"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator RecycleAudioSource(AudioClip audioClip, GameObject obj, AudioSource audioSource,
            Action<AudioSource> callback = null, float time = 0f)
        {
            // 延迟 Clip的长度（秒）
            yield return new WaitForSeconds(audioClip.length);
            yield return new WaitForSeconds(time);
            //可能外部需要拿到AudioSource做一些操作
            //用回调放回source
            callback?.Invoke(audioSource);
            if (obj != null)
            {
                // 移除引用 放回池子
                PoolMgr.PushGameObj("AudioSource", obj);
            }
        }

        #endregion 音效

        /// <summary>
        /// 释放所有AudioSource
        /// </summary>
        public void ClearAllAudioSource()
        {
            if (soundMusicList.Count == 0) return;

            for (int i = soundMusicList.Count - 1; i >= 0; i--)
            {
                ResMgr.Release(soundMusicList[i].clip);
                ResMgr.ReleaseInstance(soundMusicList[i].gameObject);
                soundMusicList.RemoveAt(i);
            }

            PoolMgr.ClearGameObject("AudioSource");
        }
    }
}