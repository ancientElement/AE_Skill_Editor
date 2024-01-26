using AE_Framework;
using MeshBuilder;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class RingLikeDetection : AttackDetectionBase
    {
        public RingLikeDetectionParams ringLikeDetectionParams;

        [SerializeField] Collider[] detectionResult;

        private void FixedUpdate()
        {
            Detection(ringLikeDetectionParams);
        }

        private void OnDrawGizmos()
        {
            if (!isDebuge || ringLikeDetectionParams == null) return;

            Vector3 center = transform.TransformPoint(new Vector3(0, -ringLikeDetectionParams.height / 2, 0));

            Vector3 forward = transform.TransformPoint(new Vector3(0, -ringLikeDetectionParams.height / 2, 1)) - center;

            Vector3[] BoxCenterAndHalfExtents = GetBoxCenterAndHalfExtents(ringLikeDetectionParams.thetaLength, ringLikeDetectionParams.height, ringLikeDetectionParams.outerRadius, ringLikeDetectionParams.innerRadius, center, forward);

            Gizmos.color = Color.green;

            Matrix4x4 oldMat = Handles.matrix;
            //获取目标旋转矩阵
            //设置当前为旋转矩阵
            Handles.matrix = transform.localToWorldMatrix;

            //这里的center是相对目标中心而言，因为旋转cube与目标位置相同所以是zero
            Handles.color = Color.green;
            Handles.DrawWireCube(transform.InverseTransformPoint(BoxCenterAndHalfExtents[0]), BoxCenterAndHalfExtents[1] * 2);

            //重置当前矩阵
            Handles.matrix = oldMat;

            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawMesh(mesh,transform.localPosition,transform.localRotation);
        }

        public override void ResetData()
        {
            detectionResult = null;
            ringLikeDetectionParams = null;
        }

        public override void Init(DetectionParamsBase detectionParamsBase)
        {
            base.Init(detectionParamsBase);
            this.ringLikeDetectionParams = (RingLikeDetectionParams)detectionParamsBase;
            if (isDebuge)
                CreatMesh();
        }

        public override void CreatMesh()
        {
            if (ringLikeDetectionParams == null) return;

            ringLikeDetectionParams.thetaStart = (Mathf.PI - ringLikeDetectionParams.thetaLength) / 2f;

            ringLikeDetectionParams.innerRadius = Mathf.Max(0, ringLikeDetectionParams.innerRadius);
            ringLikeDetectionParams.outerRadius = Mathf.Max(ringLikeDetectionParams.innerRadius, ringLikeDetectionParams.outerRadius);

            this.mesh = RingLikeBuilder.Build(
              ringLikeDetectionParams.innerRadius,
              ringLikeDetectionParams.outerRadius,
              ringLikeDetectionParams.thetaSegments,
              ringLikeDetectionParams.radiusSegments,
              ringLikeDetectionParams.thetaStart,
              ringLikeDetectionParams.thetaLength,
              ringLikeDetectionParams.height
            );
        }

        public void Detection(DetectionParamsBase detectionParamsBase)
        {
            if (ringLikeDetectionParams == null) return;

            Vector3 center = transform.TransformPoint(new Vector3(0, -ringLikeDetectionParams.height / 2, 0));

            Vector3 forward = transform.TransformPoint(new Vector3(0, -ringLikeDetectionParams.height / 2, 1)) - center;

            Vector3[] BoxCenterAndHalfExtents = GetBoxCenterAndHalfExtents(ringLikeDetectionParams.thetaLength, ringLikeDetectionParams.height, ringLikeDetectionParams.outerRadius, ringLikeDetectionParams.innerRadius, center, forward);

            Collider[] temp_detectionResult = Physics.OverlapBox(BoxCenterAndHalfExtents[0], BoxCenterAndHalfExtents[1], transform.rotation);

            if (temp_detectionResult == null || temp_detectionResult.Length == 0) { detectionResult = temp_detectionResult; return; }

            for (int i = 0; i < temp_detectionResult.Length; i++)
            {
                if (temp_detectionResult[i] == null) continue;

                Vector3 targetPosition = temp_detectionResult[i].ClosestPoint(BoxCenterAndHalfExtents[0]);

                Vector3 direction = targetPosition - center;

                if (isDebuge)
                    Debug.DrawRay(center, direction, Color.red);

                //角度剔除
                float angle = Vector3.Angle(direction, forward);

                if (angle / 180 * Mathf.PI > ringLikeDetectionParams.thetaLength / 2)
                {
                    temp_detectionResult[i] = null;
                }

                //内径剔除
                if (Vector3.Distance(targetPosition, center) < ringLikeDetectionParams.innerRadius)
                {
                    temp_detectionResult[i] = null;
                }

                //四个角剔除
                if (Vector3.Distance(targetPosition, center) > ringLikeDetectionParams.outerRadius)
                {
                    temp_detectionResult[i] = null;
                }

                if (detectionResult == null)
                    EventCenter.TriggerEvent<Collider>("BeAttacked", temp_detectionResult[i]);
                else if (temp_detectionResult[i] != null && !detectionResult.Contains(temp_detectionResult[i]))
                    EventCenter.TriggerEvent<Collider>("BeAttacked", temp_detectionResult[i]);
            }
            detectionResult = temp_detectionResult;
        }

        private Vector3[] GetBoxCenterAndHalfExtents(float fullAngle, float heigh, float outerRadius, float innerRadius, Vector3 center, Vector3 forward)
        {
            Vector3 boxCenter = Vector3.zero;

            float halfDepth = 0;
            float halfWidth = 0;
            float halfHeight = heigh / 2;

            float halfAngle = fullAngle / 2;//弧度

            float cosHalfAngle = Mathf.Abs(Mathf.Cos(halfAngle));
            float sinHalfAngle = Mathf.Sin(halfAngle);

            if (ringLikeDetectionParams.thetaLength < Mathf.PI)
            {
                halfDepth = (outerRadius - cosHalfAngle * innerRadius) / 2;

                halfWidth = outerRadius * sinHalfAngle;

                boxCenter = center + forward.normalized * (halfDepth + innerRadius * cosHalfAngle);
            }
            else
            {
                halfDepth = (outerRadius + (cosHalfAngle * outerRadius)) / 2;

                halfWidth = outerRadius;

                boxCenter = center + forward.normalized * (outerRadius - halfDepth);
            }

            Vector3 halfExtents = new Vector3(halfWidth, halfHeight, halfDepth);

            return new Vector3[] { boxCenter, halfExtents };
        }
    }
}