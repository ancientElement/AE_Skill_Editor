using AE_Framework;
using System.Collections;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AttackDetectionDriver
    {
        public static void Driver(SkillMultiLineTrackDataBase<AttackDetectionEvent> attackDetectionData, int currentFrameIndex, Transform modelTransfrom,int frameRate,bool debugMode)
        {
            if (attackDetectionData != null)
            {
                foreach (AttackDetectionEvent attackDetectionEvent in attackDetectionData.FrameData)
                {
                    if (attackDetectionEvent.DetectionParamsBase != null)
                    {
                        if (attackDetectionEvent.FrameIndex == currentFrameIndex)
                        {
                            string name = attackDetectionEvent.DetectionParamsBase.GetType().Name;

                            GameObject attackDetectionObj;

                            //实例化特效
                            attackDetectionObj = PoolMgr.GetGameObj(name);

                            AttackDetectionBase demo;

                            if (attackDetectionObj == null)
                            {
                                attackDetectionObj = new GameObject(name);

                                if (attackDetectionEvent.DetectionParamsBase is RingLikeDetectionParams)
                                    demo = attackDetectionObj.AddComponent<RingLikeDetection>();
                                else if (attackDetectionEvent.DetectionParamsBase is CubeDetectionParams)
                                    demo = attackDetectionObj.AddComponent<CubeDetection>();
                                else
                                    demo = attackDetectionObj.AddComponent<SphereDetection>();
                            }
                            else
                            {
                                if (attackDetectionEvent.DetectionParamsBase is RingLikeDetectionParams)
                                    demo = attackDetectionObj.GetComponent<RingLikeDetection>();
                                else if (attackDetectionEvent.DetectionParamsBase is CubeDetectionParams)
                                    demo = attackDetectionObj.GetComponent<CubeDetection>();
                                else
                                    demo = attackDetectionObj.GetComponent<SphereDetection>();
                            }

                            demo.Init(attackDetectionEvent.DetectionParamsBase);
                            demo.isDebuge = !Application.isPlaying;

                            attackDetectionObj.transform.position = modelTransfrom.TransformPoint(attackDetectionEvent.Position);
                            attackDetectionObj.transform.rotation = Quaternion.Euler(modelTransfrom.eulerAngles + attackDetectionEvent.Rotation);
                            MonoMgr.Instance.StartCoroutine(AutoDestroyGameObject(((float)attackDetectionEvent.FrameDuration / (float)frameRate), attackDetectionObj, name));
                        }
                    }
                }
            }
        }

        public static IEnumerator AutoDestroyGameObject(float time, GameObject obj, string name)
        {
            yield return new WaitForSeconds(time);
            PoolMgr.PushGameObj(name, obj);
        }
    }
}
