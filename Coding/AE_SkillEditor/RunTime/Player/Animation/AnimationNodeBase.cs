using AE_Framework;

namespace ARPG_AE_JOKER.SkillEditor
{
    public abstract class AnimationNodeBase
    {
        public int InputPort;

        public virtual void PushPool()
        {
            PoolMgr.PushObj<SingleAnimationNode>(this);
        }

        public virtual void SetSpeed(float speed)
        { }
    }
}