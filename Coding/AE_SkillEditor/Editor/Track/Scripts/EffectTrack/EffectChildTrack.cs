using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class EffectChildTrack : MultiLineChildTrack<SkillEffectEvent, EffectTrackItem>
    {
        public float ItemFrameCount
        { get { return ItemData.EffectObject.GetComponent<ParticleSystem>().main.duration * SkillEditorWindow.Instance.SkillConfig.FrameRate; } }

        public override void Init(MultiLineTrack m_ParentTrack, SkillMultiLineTrackStyle parentStyle, float frameWidth, int index, Action<int> DelectChildTrackAction, SkillEffectEvent m_ItemData)
        {
            base.Init(m_ParentTrack, parentStyle, frameWidth, index, DelectChildTrackAction, m_ItemData);
        }

        /// <summary>
        /// 驱动显示
        /// </summary>
        /// <param name="currentFrameIndex"></param>
        public override void TickView(int currentFrameIndex)
        {
            if (m_TrackItem != null)
                if (m_TrackItem.ItemData.EffectObject != null)
                    m_TrackItem.TickView(currentFrameIndex);
        }

        public override string[] GetObjectType()
        {
            return new string[] { typeof(GameObject).FullName };
        }

        public override int CaculateObjectLength(object clip)
        {
            ParticleSystem[] particleSystems = (clip as GameObject).GetComponentsInChildren<ParticleSystem>();
            float maxvalue = 0;
            int max = 0;
            for (int i = 0; i < particleSystems.Length; i++)
            {
                if (particleSystems[i].main.duration > maxvalue)
                {
                    max = i;
                    maxvalue = particleSystems[i].main.duration;
                }
            }
            //data
            if (m_ItemData.FrameDuration == -1)
            {
                m_ItemData.FrameDuration = (int)(particleSystems[max].main.duration * (float)SkillEditorWindow.Instance.SkillConfig.FrameRate);
            }
            //save
            SkillEditorWindow.Instance.AutoSaveConfig();
            return (int)(m_ItemData.FrameDuration);
        }
    }
}