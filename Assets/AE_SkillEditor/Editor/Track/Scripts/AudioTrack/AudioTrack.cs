using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AudioTrack : MultiLineTrack<SkillAudioEvent, AudioChildTrack>
    {
        private GameObject audioSouceGameObj;
        public GameObject AudioSouceGameObj
        {
            get
            {
                if (audioSouceGameObj == null)
                {
                    audioSouceGameObj = GameObject.Find("AudioSouceGameObj");
                    if (audioSouceGameObj == null)
                        audioSouceGameObj = new GameObject("AudioSouceGameObj");
                }
                return audioSouceGameObj;
            }
        }

        public static string TrackName = "音频轨道";

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth, SkillTrackDataBase m_Data, string name)
        {
            base.Init(menuParent, trackParent, frameWidth, m_Data, name);


        }

        #region 预览

        public override void OnPlay(int startFrame)
        {
            base.OnPlay(startFrame);
        }

        public override void TickView(int currentFrameIndex)
        {
            if (SkillEditorWindow.Instance.IsPlaying)
            {
                foreach (SkillAudioEvent audioEvent in Data.FrameData)
                {
                    if (audioEvent.AudioClip != null)
                    {
                        if (audioEvent.FrameIndex == currentFrameIndex)
                        {
                            PlayAudio(AudioSouceGameObj, audioEvent.AudioClip, audioEvent.volumn);
                        }
                    }
                }
            }
        }

        public void PlayAudio(GameObject AudioSouceGameObj, AudioClip audioClip, float volume)
        {
            AudioSource audioSource = AudioSouceGameObj.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.loop = false;
            audioSource.Play();
            SkillEditorWindow.Instance.StartCoroutine(DestoryAudioSouce(audioSource, audioClip.length));
        }

        public IEnumerator DestoryAudioSouce(AudioSource audioSource, float time)
        {
            yield return new EditorWaitForSeconds(time);
            GameObject.DestroyImmediate(audioSource);
        }

        #endregion 预览
    }
}