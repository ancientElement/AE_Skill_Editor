using UnityEditor;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    //[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class AttackDetectionBase : MonoBehaviour
    {
        public Mesh mesh;

        public bool isDebuge;

        public bool isAwaking;

        private void OnDisable()
        {
            ResetData();
        }

        public virtual void Init(DetectionParamsBase detectionParamsBase) { }

        public virtual void CreatMesh() { }

        public virtual void ResetData() { }
    }
}
