using System;
using UnityEngine;

namespace AE_Framework
{
    /// <summary>
    /// 可二进制转换的Color
    /// </summary>
    [Serializable]
    public struct Serialization_Color
    {
        public float r, g, b, a;

        public Serialization_Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            return $"({r},{g},{b},{a})";
        }

        public override int GetHashCode()
        {
            return this.ConverToUnityColor().GetHashCode();
        }
    }

    public static class Serialization_ColorExtensions
    {
        /// <summary>
        /// unity Color 转 可序列化Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ConverToUnityColor(this Serialization_Color color)
        {
            return new Color(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// 可序列化Color 转 unity Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Serialization_Color ConverToSerializationColor(this Color color)
        {
            return new Serialization_Color(color.r, color.g, color.b, color.a);
        }
    }
}