using AE_Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class EffectDriver
    {
       public static void Drive(SkillMultiLineTrackDataBase<SkillEffectEvent> EffectData, int currentFrameIndex, Transform modelTransfrom, int frameRate)
        {
            if (EffectData != null)
            {
                foreach (SkillEffectEvent effectEvent in EffectData.FrameData)
                {
                    if (effectEvent.EffectObject != null)
                    {
                        if (effectEvent.FrameIndex == currentFrameIndex)
                        {
                            //实例化特效
                            GameObject effct = PoolMgr.GetGameObjNotInRes(effectEvent.EffectObject.name);

                            if (effct == null)
                            {
                                effct = GameObject.Instantiate(effectEvent.EffectObject);
                            }

                            effct.transform.position = modelTransfrom.TransformPoint(effectEvent.Position);
                            effct.transform.rotation = Quaternion.Euler(modelTransfrom.eulerAngles + effectEvent.Rotation);
                            effct.transform.localScale = effectEvent.Scale;
                            if (effectEvent.AutoDestroy)
                            {
                                MonoMgr.Instance.StartCoroutine(AutoDestroyGameObject(((float)effectEvent.FrameDuration / (float)frameRate), effct, effectEvent.EffectObject.name));
                            }
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
