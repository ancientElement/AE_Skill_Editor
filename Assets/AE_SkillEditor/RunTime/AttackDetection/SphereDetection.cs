using AE_Framework;
using MeshBuilder;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class SphereDetection : AttackDetectionBase
    {
        public SphereDetectionParams sphereDetectionParams;

        public override void Init(DetectionParamsBase detectionParamsBase)
        {
            base.Init(detectionParamsBase);
            this.sphereDetectionParams = (SphereDetectionParams)detectionParamsBase;
            if (isDebuge)
                CreatMesh();
        }

        [SerializeField] Collider[] detectionResult;

        private void FixedUpdate()
        {
            Detection(sphereDetectionParams);
        }

        private void OnDrawGizmos()
        {
            if (!isDebuge || sphereDetectionParams == null) return;

            Vector3 sphereCenter = transform.position;
            float raduis = sphereDetectionParams.radius;

            Handles.color = Color.green;
            Handles.DrawWireDisc(sphereCenter, Vector3.up, raduis);
            Handles.DrawWireDisc(sphereCenter, Vector3.right, raduis);
            Handles.DrawWireDisc(sphereCenter, Vector3.forward, raduis);

            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawMesh(mesh, transform.localPosition, transform.localRotation);
        }

        public void Detection(DetectionParamsBase detectionParamsBase)
        {
            if (sphereDetectionParams == null) return;

            Vector3 sphereCenter = transform.position;
            float raduis = sphereDetectionParams.radius;

            Collider[] temp_detectionResult = Physics.OverlapSphere(sphereCenter, raduis);


            for (int i = 0; i < temp_detectionResult.Length; i++)
            {
                if (detectionResult == null)
                    EventCenter.TriggerEvent<Collider>("BeAttacked", temp_detectionResult[i]);
                else if (temp_detectionResult[i] != null && !detectionResult.Contains(temp_detectionResult[i]))
                    EventCenter.TriggerEvent<Collider>("BeAttacked", temp_detectionResult[i]);
            }

            detectionResult = temp_detectionResult;
        }

        public override void CreatMesh()
        {
            if (sphereDetectionParams == null) return;

            mesh = SphereBuilder.Build(
              sphereDetectionParams.radius,
              sphereDetectionParams.lonSegments,
              sphereDetectionParams.latSegments
            );
        }

        public override void ResetData()
        {
            sphereDetectionParams = null;
            detectionResult = null;
        }
    }
}