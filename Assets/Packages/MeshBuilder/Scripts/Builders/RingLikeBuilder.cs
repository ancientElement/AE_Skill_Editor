using UnityEngine;
using System.Collections.Generic;
namespace MeshBuilder
{

    public class RingLikeBuilder : MeshBuilderBase
    {
        public static Mesh Build(float innerRadius, float outerRadius, int thetaSegments, int radiusSegments, float thetaStart, float thetaLength, float height)
        {
            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();

            //左侧
            List<int> asideLeftIndex = new List<int>();
            //右侧
            List<int> asideRightIndex = new List<int>();
            //前侧
            List<int> asideForwardIndex = new List<int>();
            //后侧
            List<int> asideBackIndex = new List<int>();

            thetaSegments = Mathf.Max(3, thetaSegments);
            radiusSegments = Mathf.Max(1, radiusSegments);

            var radiusStep = ((outerRadius - innerRadius) / radiusSegments);
            var radius = innerRadius;

            //上面顶点
            for (int j = 0; j <= radiusSegments; j++)
            {
                for (int i = 0; i <= thetaSegments; i++)
                {
                    float startTheta = thetaStart + 1f * i / thetaSegments * thetaLength;
                    Vector3 vertex = new Vector3(
                        radius * Mathf.Cos(startTheta),
                         0f,
                        radius * Mathf.Sin(startTheta)
                    );
                    vertices.Add(vertex);
                    //左侧
                    if (i == thetaSegments)
                    {
                        asideLeftIndex.Add(vertices.Count - 1);
                    }
                    //右侧
                    if (i == 0)
                    {
                        asideRightIndex.Add(vertices.Count - 1);
                    }
                    //前侧
                    if (j == radiusSegments)
                    {
                        asideForwardIndex.Add(vertices.Count - 1);
                    }
                    //后侧
                    if (j == 0)
                    {
                        asideBackIndex.Add(vertices.Count - 1);
                    }
                }
                radius += radiusStep;
            }

            //上面三角形
            for (int j = 0; j < radiusSegments; j++)
            {
                var thetaSegmentLevel = j * (thetaSegments + 1);
                for (int i = 0; i < thetaSegments; i++)
                {
                    var segment = i + thetaSegmentLevel;
                    var a = segment;
                    var b = segment + thetaSegments + 1;
                    var c = segment + thetaSegments + 2;
                    var d = segment + 1;
                    triangles.Add(a); triangles.Add(d); triangles.Add(b);
                    triangles.Add(b); triangles.Add(d); triangles.Add(c);
                }
            }

            radiusStep = ((outerRadius - innerRadius) / radiusSegments);
            radius = innerRadius;

            //下面顶点
            for (int j = 0; j <= radiusSegments; j++)
            {
                for (int i = 0; i <= thetaSegments; i++)
                {
                    float startTheta = thetaStart + 1f * i / thetaSegments * thetaLength;
                    Vector3 vertex = new Vector3(
                        radius * Mathf.Cos(startTheta),
                         -height,
                        radius * Mathf.Sin(startTheta)
                    );
                    vertices.Add(vertex);
                    //左侧
                    if (i == thetaSegments)
                    {
                        asideLeftIndex.Add(vertices.Count - 1);
                    }
                    //右侧
                    if (i == 0)
                    {
                        asideRightIndex.Add(vertices.Count - 1);
                    }
                    //前侧
                    if (j == radiusSegments)
                    {
                        asideForwardIndex.Add(vertices.Count - 1);
                    }
                    //后侧
                    if (j == 0)
                    {
                        asideBackIndex.Add(vertices.Count - 1);
                    }
                }
                radius += radiusStep;
            }

            //下面三角形
            for (int j = radiusSegments + 1; j <= radiusSegments * 2; j++)
            {
                var thetaSegmentLevel = j * (thetaSegments + 1);
                for (int i = 0; i < thetaSegments; i++)
                {
                    var segment = i + thetaSegmentLevel;
                    var a = segment;
                    var b = segment + thetaSegments + 1;
                    var c = segment + thetaSegments + 2;
                    var d = segment + 1;
                    triangles.Add(a); triangles.Add(b); triangles.Add(d);
                    triangles.Add(b); triangles.Add(c); triangles.Add(d);
                }
            }

            //右侧三角形
            for (int i = 0; i < radiusSegments; i++)
            {
                var a = asideRightIndex[i];
                var b = asideRightIndex[i + radiusSegments + 1];
                var c = asideRightIndex[i + radiusSegments + 2];
                var d = asideRightIndex[i + 1];
                triangles.Add(a); triangles.Add(d); triangles.Add(b);
                triangles.Add(b); triangles.Add(d); triangles.Add(c);
            }

            //左侧
            for (int i = 0; i < radiusSegments; i++)
            {
                var a = asideLeftIndex[i];
                var b = asideLeftIndex[i + radiusSegments + 1];
                var c = asideLeftIndex[i + radiusSegments + 2];
                var d = asideLeftIndex[i + 1];
                triangles.Add(a); triangles.Add(b); triangles.Add(d);
                triangles.Add(b); triangles.Add(c); triangles.Add(d);
            }

            //前侧
            for (int i = 0; i < thetaSegments; i++)
            {
                var a = asideForwardIndex[i];
                var b = asideForwardIndex[i + thetaSegments + 1];
                var c = asideForwardIndex[i + thetaSegments + 2];
                var d = asideForwardIndex[i + 1];
                triangles.Add(a); triangles.Add(d); triangles.Add(b);
                triangles.Add(b); triangles.Add(d); triangles.Add(c);
            }

            //后侧
            if (innerRadius != 0)
                for (int i = 0; i < thetaSegments; i++)
                {
                    var a = asideBackIndex[i];
                    var b = asideBackIndex[i + thetaSegments + 1];
                    var c = asideBackIndex[i + thetaSegments + 2];
                    var d = asideBackIndex[i + 1];
                    triangles.Add(a); triangles.Add(b); triangles.Add(d);
                    triangles.Add(b); triangles.Add(c); triangles.Add(d);
                }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}