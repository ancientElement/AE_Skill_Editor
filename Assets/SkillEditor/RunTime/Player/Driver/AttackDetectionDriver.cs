using AE_Framework;
using System.Collections;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class AttackDetectionDriver
    {
        public static void Driver(SkillMultiLineTrackDataBase<AttackDetectionEvent> attackDetectionData, int currentFrameIndex, Transform modelTransfrom,int frameRate)
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
                            attackDetectionObj = PoolMgr.GetGameObjNotInRes(name);

                            if (attackDetectionObj == null)
                            {
                                attackDetectionObj = new GameObject(name);

                                if (attackDetectionEvent.DetectionParamsBase is RingLikeDetectionParams)
                                {
                                    RingLikeDetection demo = attackDetectionObj.AddComponent<RingLikeDetection>();
                                    demo.isDebuge = true;
                                    demo.Init(attackDetectionEvent.DetectionParamsBase as RingLikeDetectionParams);
                                }
                                else if (attackDetectionEvent.DetectionParamsBase is CubeDetectionParams)
                                {
                                    CubeDetection demo = attackDetectionObj.AddComponent<CubeDetection>();
                                    demo.isDebuge = true;
                                    demo.Init(attackDetectionEvent.DetectionParamsBase as CubeDetectionParams);
                                }
                                else
                                {
                                    SphereDetection demo = attackDetectionObj.AddComponent<SphereDetection>();
                                    demo.isDebuge = true;
                                    demo.Init(attackDetectionEvent.DetectionParamsBase as SphereDetectionParams);
                                }
                            }
                            else
                            {
                                if (attackDetectionEvent.DetectionParamsBase is RingLikeDetectionParams)
                                {
                                    RingLikeDetection demo = attackDetectionObj.GetComponent<RingLikeDetection>();
                                    demo.isDebuge = true;
                                    demo.Init(attackDetectionEvent.DetectionParamsBase as RingLikeDetectionParams);
                                }
                                else if (attackDetectionEvent.DetectionParamsBase is CubeDetectionParams)
                                {
                                    CubeDetection demo = attackDetectionObj.GetComponent<CubeDetection>();
                                    demo.isDebuge = true;
                                    demo.Init(attackDetectionEvent.DetectionParamsBase as CubeDetectionParams);
                                }
                                else
                                {
                                    SphereDetection demo = attackDetectionObj.GetComponent<SphereDetection>();
                                    demo.isDebuge = true;
                                    demo.Init(attackDetectionEvent.DetectionParamsBase as SphereDetectionParams);
                                }
                            }

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
