using UnityEditor;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class AttackDetectionBase : MonoBehaviour
    {
        public Mesh mesh;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public bool isDebuge;

        public bool isAwaking;

        private void Awake()
        {
            isAwaking = true;
        }

        public void Start()
        {
            isAwaking = false;
        }

        private void OnDisable()
        {
            ResetData();
        }

        protected void OnValidate()
        {
            if (!isAwaking)
            {
                if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
                Check();
                if (isDebuge)
                    DrawMesh(meshFilter);
            }
        }

        private void Check()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }

            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            if (meshFilter == null || meshRenderer == null) return;

            if (meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Packages/MeshBuilder/Materials/Transparent.mat");
            }
        }

        public virtual void Init(DetectionParamsBase detectionParamsBase)
        {
            Check();
        }

        public virtual void DrawMesh(MeshFilter meshFilter) { }

        public virtual void ResetData() { }
    }
}
