using UnityEngine;

namespace AE_Animation_Playable
{
    [CreateAssetMenu(fileName = "SingleAnimClipSO", menuName = "AE_Animation_Playable/SingleAnimClipSO")]
    public class SingleAnimClipSO : AnimSO
    {
        private void OnEnable()
        {
            AnimType = AnimTypeEnum.SingleAnimClip;
        }
        public AnimationClip AnimationClip;
    }
}
