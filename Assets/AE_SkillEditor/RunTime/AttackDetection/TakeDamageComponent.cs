using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class TakeDamageComponent : MonoBehaviour
    {
        public void TakeDamage()
        {
            Debug.Log(name + "Get Damage");
        }
        public void TakeDamage(int damageValue)
        {
            Debug.Log(damageValue);
        }
    }
}
