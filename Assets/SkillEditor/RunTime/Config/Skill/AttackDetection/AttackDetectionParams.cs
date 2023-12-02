using System;
using UnityEngine;

namespace ARPG_AE_JOKER.SkillEditor
{
    public class DetectionParamsBase
    {

    }

    [Serializable]
    public class RingLikeDetectionParams : DetectionParamsBase
    {
        public float innerRadius;
        public float outerRadius;
        public float thetaStart;
        [Range(0, 6.29f)] public float thetaLength;
        public float height;
        [HideInInspector] public int thetaSegments = 60;
        [HideInInspector] public int radiusSegments = 2;
    }

    [Serializable]
    public class CubeDetectionParams : DetectionParamsBase
    {
        public float width;
        public float height;
        public float depth;
        [HideInInspector] public int widthSegments = 1;
        [HideInInspector] public int heightSegments = 1;
        [HideInInspector] public int depthSegments = 1;
    }

    [Serializable]
    public class SphereDetectionParams : DetectionParamsBase
    {
        public float radius;
        [HideInInspector] public int lonSegments = 30;
        [HideInInspector] public int latSegments = 30;
    }
}
