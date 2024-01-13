using System;
using System.Reflection;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public static class EditorAudioUtility
    {
        private static MethodInfo playAudioClipMethod;
        private static MethodInfo stopAudioClipMethod;

        static EditorAudioUtility()
        {
            //UnityEditor.AudioUtil
            Assembly unityEditor = typeof(UnityEditor.AudioImporter).Assembly;
            Type audioUtil = unityEditor.GetType("UnityEditor.AudioUtil");
            playAudioClipMethod = audioUtil.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null, new Type[3] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
            stopAudioClipMethod = audioUtil.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="start"></param>
        public static void PlayAudio(AudioClip audioClip, float start)
        {
            playAudioClipMethod.Invoke(null, new object[] { audioClip, (int)(start * audioClip.frequency * SkillEditorWindow.Instance.animationSpeed), false });
        }

        /// <summary>
        /// 结束音效
        /// </summary>
        public static void StopAllAudio()
        {
            stopAudioClipMethod.Invoke(null, null);
        }
    }
}