using UnityEngine;
using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    public class CustomAnimBehaviourSO : AnimSO
    {
        protected virtual void OnEnable()
        {
            AnimType = AnimTypeEnum.CoustimAnimBehaviour;
        }
        public virtual void Init(PlayableGraph graph) { }
        public AnimBehaviour AnimBehaviour;
    }
}
