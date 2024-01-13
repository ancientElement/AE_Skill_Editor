using System.Collections.Generic;
using UnityEngine;
namespace ARPG_AE_JOKER.SkillEditor
{
    [CreateAssetMenu(fileName = "事件列表", menuName = "AE技能编辑器/事件列表")]
    public class EventNameListSO : ScriptableObject
    {
        public List<string> eventNameList = new List<string>();
    }
}