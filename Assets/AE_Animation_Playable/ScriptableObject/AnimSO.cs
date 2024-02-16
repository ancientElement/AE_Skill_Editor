using UnityEngine;

namespace AE_Animation_Playable
{
    public class AnimSO : ScriptableObject
    {
        public enum AnimTypeEnum
        {
            SingleAnimClip,
            CoustimAnimBehaviour,
            BlendTree1D,
            BlendTree2D,
        }
        public AnimTypeEnum AnimType;
        public string AnimName;
        private void OnValidate()
        {
            AnimName = name;
        }
    }
}
