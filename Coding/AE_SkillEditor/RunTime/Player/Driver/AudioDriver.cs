using UnityEngine;
using AE_Framework;
using Unity.EditorCoroutines.Editor;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AudioDriver
    {
        public static void Drive(SkillMultiLineTrackDataBase<SkillAudioEvent> AudioData, int currentFrameIndex, Vector3 position)
        {
            if (AudioData != null)
            {
                foreach (SkillAudioEvent audioEvent in AudioData.FrameData)
                {
                    if (audioEvent.AudioClip != null)
                    {
                        if (audioEvent.FrameIndex == currentFrameIndex)
                        {
                            MusicMgr.Instance.PlaySoundMusic(audioEvent.AudioClip, position, volumn: audioEvent.volumn);
                        }
                    }
                }
            }
        }
    }
}
