using System;
using UnityEngine;

namespace AE_Framework
{
    /// <summary>
    /// 可序列化的Vector3
    /// </summary>
    [Serializable]
    public class Serialization_Vector3
    {
        public float x, y, z;

        public Serialization_Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }

        public override int GetHashCode()
        {
            return this.ConverToUnityVector3().GetHashCode();
        }
    }

    /// <summary>
    /// 可序列化的Vector3
    /// </summary>
    [Serializable]
    public class Serialization_Vector2
    {
        public float x, y;

        public Serialization_Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public override int GetHashCode()
        {
            return this.ConverToUnityVector2().GetHashCode();
        }
    }

    public static class Serialization_VectorExtensions
    {
        /// <summary>
        /// 可序列化 转 Vector3unity Vector3
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vector3 ConverToUnityVector3(this Serialization_Vector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }

        /// <summary>
        /// unity Vector3 转 可序列化Vector3
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Serialization_Vector3 ConverToSerializationVector3(this Vector3 vector3)
        {
            return new Serialization_Vector3(vector3.x, vector3.y, vector3.z);
        }

        /// <summary>
        /// 可序列化Vector3 转 unity Vector3Int
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vector3 ConverToUnityVector3Int(this Serialization_Vector3 vector3)
        {
            return new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
        }

        /// <summary>
        /// 可序列化 转 Vector2unity Vector2
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vector2 ConverToUnityVector2(this Serialization_Vector2 vector2)
        {
            return new Vector2(vector2.x, vector2.y);
        }

        /// <summary>
        /// unity Vector2 转 可序列化Vector2
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Serialization_Vector2 ConverToSerializationVector3(this Vector2 vector2)
        {
            return new Serialization_Vector2(vector2.x, vector2.y);
        }

        /// <summary>
        /// 可序列化Vector2 转 unity Vector2Int
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vector2 ConverToUnityVector2Int(this Serialization_Vector2 vector2)
        {
            return new Vector2Int((int)vector2.x, (int)vector2.y);
        }
    }
}