using UnityEngine.Playables;

namespace AE_Animation_Playable
{
    /// <summary>
    /// 随机动画播放
    /// </summary>
    public class RandomSelector : AnimSelector
    {
        public RandomSelector(PlayableGraph playable, float enterTime) : base(playable, enterTime)
        {
        }

        public override int Select()
        {
            currentIndex = UnityEngine.Random.Range(0, clipCount);
            return currentIndex;
        }
    }
}