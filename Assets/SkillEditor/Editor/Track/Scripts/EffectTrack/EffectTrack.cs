using UnityEngine;
using UnityEngine.UIElements;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class EffectTrack : MultiLineTrack<SkillEffectEvent, EffectChildTrack>
    {
        public static string TrackName = "特效轨道";

        //特效物体父对象
        private static GameObject _tempEffectPriview;

        public static GameObject TempEffectPriview
        {
            get
            {
                if (SkillEditorWindow.Instance.isOnEditorScene)
                {
                    if (_tempEffectPriview == null)
                    {
                        _tempEffectPriview = GameObject.Find("Effects");
                        if(_tempEffectPriview == null)
                        {
                            _tempEffectPriview = new GameObject("Effects");
                            _tempEffectPriview.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                        }
                    }
                    return _tempEffectPriview;
                }
                Debug.LogWarning("不在编辑器场景下");
                return null;
            }
            private set { _tempEffectPriview = value; }
        }

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth, SkillTrackDataBase m_Data, string name)
        {
            base.Init(menuParent, trackParent, frameWidth, m_Data, name);

            DestroyAndReload(frameWidth);
        }

        public override void OnPlay(int startFrame)
        {
            base.OnPlay(startFrame);
        }

        /// <summary>
        /// 驱动显示
        /// </summary>
        /// <param name="currentFrameIndex"></param>
        public override void TickView(int currentFrameIndex)
        {
            foreach (EffectChildTrack item in m_ChildTrackList)
            {
                item.TickView(currentFrameIndex);
            }
        }

        public override void OnStop()
        {
        }

        public override void ClearScene()
        {
            base.ClearScene();
            for (int i = TempEffectPriview.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(TempEffectPriview.transform.GetChild(i).gameObject);
            }
        }
    }
}