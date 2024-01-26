using AE_Framework;
using MeshBuilder;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class CubeDetection : AttackDetectionBase
    {
        public CubeDetectionParams cubeDetectionParams;

        public override void Init(DetectionParamsBase detectionParamsBase)
        {
            base.Init(detectionParamsBase);
            this.cubeDetectionParams = (CubeDetectionParams)detectionParamsBase;
            if (isDebuge)
                CreatMesh();
        }

        [SerializeField] Collider[] detectionResult;
        Vector3 boxCenter;
        Vector3 halfExtents;

        private void FixedUpdate()
        {
            Detection(cubeDetectionParams);
        }

        private void OnDrawGizmos()
        {
            if (!isDebuge || cubeDetectionParams == null) return;

            Vector3 boxCenter = transform.position;
            Vector3 halfExtents = new Vector3(cubeDetectionParams.width / 2, cubeDetectionParams.height / 2, cubeDetectionParams.depth / 2);

            Matrix4x4 oldMat = Handles.matrix;
            //获取目标旋转矩阵
            //设置当前为旋转矩阵
            Handles.matrix = transform.localToWorldMatrix;

            //这里的center是相对目标中心而言，因为旋转cube与目标位置相同所以是zero
            Handles.color = Color.green;
            Handles.DrawWireCube(transform.InverseTransformPoint(boxCenter), halfExtents * 2);
            Handles.matrix = oldMat;

            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawMesh(mesh, transform.localPosition, transform.localRotation);
        }

        public void Detection(DetectionParamsBase detectionParamsBase)
        {
            if (cubeDetectionParams == null) return;

            boxCenter = transform.position;
            halfExtents = new Vector3(cubeDetectionParams.width / 2, cubeDetectionParams.height / 2, cubeDetectionParams.depth / 2);

            Collider[] temp_detectionResult = Physics.OverlapBox(boxCenter, halfExtents, transform.rotation);

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
            if (cubeDetectionParams == null) return;
            mesh = CubeBuilder.Build(
                  this.cubeDetectionParams.width,
                  this.cubeDetectionParams.height,
                  this.cubeDetectionParams.depth,
                  this.cubeDetectionParams.widthSegments,
                  this.cubeDetectionParams.heightSegments,
                  this.cubeDetectionParams.depthSegments
                );
        }

        public override void ResetData()
        {
            detectionResult = null;
            cubeDetectionParams = null;
        }
    }
}