using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AE_Framework
{
    public class MusicMgr : SingletonMonoMgr<MusicMgr>
    {
        [SerializeField] private GameObject MusicRoot; //音效挂载的游戏对象

        private AudioSource bkMusic; //背景音乐组件
        [SerializeField] private float bkVolume = 0.5f; //背景音乐音量

        private List<AudioSource> soundMusicList = new List<AudioSource>(); //音效组件

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
        private void SetAudioSource(AudioSource audioSource, AudioClip audioClip, float time, bool isLoop = false,
            float spatialBlend = 0, bool mute = false, float volumn = 0.5f)
        {
            audioSource.clip = audioClip;
            //设置时间
            audioSource.time = time;
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
        public void PlayBKMusic(AudioClip audioClip)
        {
            //播放背景音乐
            if (bkMusic == null)
            {
                bkMusic = MusicRoot.AddComponent<AudioSource>();
            }
            SetAudioSource(bkMusic, audioClip, 0f, isLoop: true);
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
            bkMusic.clip = null;
        }

        #endregion 背景音乐

        #region 音效

        /// <summary>
        /// 播放音效
        /// </summary>
        public AudioSource PlaySoundMusic(AudioClip audioClip, float time = 0f, bool isLoop = false,
            float spatialBlend = 0f, bool mute = false, float volumn = 0.5f)
        {
            AudioSource audioSource = MusicRoot.AddComponent<AudioSource>();
            //添加进soundMusicList
            soundMusicList.Add(audioSource);
            //设置音效
            SetAudioSource(audioSource, audioClip, time, isLoop, spatialBlend, mute, volumn);
            audioSource.Play();
            //协程回收AudioSource
            StartCoroutine(RecycleAudioSource(audioClip, audioSource));
            return audioSource;
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
        private IEnumerator RecycleAudioSource(AudioClip audioClip, AudioSource audioSource)
        {
            // 延迟 Clip的长度（秒）
            yield return new WaitForSeconds(audioClip.length);
            audioSource.Stop();
            soundMusicList.Remove(audioSource);
            GameObject.Destroy(audioSource);
        }

        #endregion 音效
    }
}