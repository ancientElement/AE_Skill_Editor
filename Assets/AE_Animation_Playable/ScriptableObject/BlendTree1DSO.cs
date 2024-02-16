using UnityEngine;

namespace AE_Animation_Playable
{
    [CreateAssetMenu(fileName = "BlendTree1D",menuName = "AE_Animation_Playable/BlendTree1D")]
    public class BlendTree1DSO : AnimSO
    {
        private void OnEnable()
        {
            AnimType = AnimTypeEnum.BlendTree1D;
        }
        public BlendClip1D[] BlendClip1DClips;
    }
}
