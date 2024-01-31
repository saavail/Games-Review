using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ChangeX(this Vector3 v, float newX)
        {
            v.x = newX;
            return v;
        }

        public static Vector2 ChangeX(this Vector2 v, float newX)
        {
            v.x = newX;
            return v;
        }

        public static Vector3 ChangeY(this Vector3 v, float newY)
        {
            v.y = newY;
            return v;
        }

        public static Vector2 ChangeY(this Vector2 v, float newY)
        {
            v.y = newY;
            return v;
        }

        public static Vector3 ChangeZ(this Vector3 v, float newZ)
        {
            v.z = newZ;
            return v;
        }

        public static Vector3 ApplyDeltaX(this Vector3 v, float deltaX)
        {
            v.x += deltaX;
            return v;
        }

        public static Vector2 ApplyDeltaX(this Vector2 v, float deltaX)
        {
            v.x += deltaX;
            return v;
        }

        public static Vector3 ApplyDeltaY(this Vector3 v, float deltaY)
        {
            v.y += deltaY;
            return v;
        }

        public static Vector2 ApplyDeltaY(this Vector2 v, float deltaY)
        {
            v.y += deltaY;
            return v;
        }

        public static Vector3 ApplyDeltaZ(this Vector3 v, float deltaZ)
        {
            v.z += deltaZ;
            return v;
        }

        public static float SignedAngle(this Vector2 from, Vector2 to)
        {
            float angle = Vector2.Angle(from, to);

            if (Vector3.Cross(from, to).z > 0)
                return angle;
            else
                return -angle;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);

            return v;
        }

        public static Vector2 ToVector2XZ(this Vector3 source)
        {
            return new Vector2(source.x, source.z);
        }

        public static Vector3 ToVector3XZ(this Vector2 source)
        {
            return new Vector3(source.x, 0f, source.y);
        }

        public static Vector2 ToVector2(this Vector3 source)
        {
            return new Vector2(source.x, source.y);
        }

        public static Vector3 ToVector3(this Vector2 source)
        {
            return new Vector3(source.x, source.y, 0f);
        }
        
        public static Vector3 MinVectorValues(this IEnumerable<Vector3> en) => 
            new Vector3(
                en.Select(v => v.x).Min(),
                en.Select(v => v.y).Min(), 
                en.Select(v => v.z).Min());
        
        public static Vector3 MaxVectorValues(this IEnumerable<Vector3> en) => 
            new Vector3(
                en.Select(v => v.x).Max(),
                en.Select(v => v.y).Max(), 
                en.Select(v => v.z).Max());
        
        public static Vector2 MinVectorValues(this IEnumerable<Vector2> en) => 
            new Vector2(
                en.Select(v => v.x).Min(),
                en.Select(v => v.y).Min());
        
        public static Vector2 MaxVectorValues(this IEnumerable<Vector2> en) => 
            new Vector2(
                en.Select(v => v.x).Max(),
                en.Select(v => v.y).Max());
        
        /// <summary>compares the squared magnitude of target - second to given float value</summary>
        public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
        {
            return (target - second).sqrMagnitude < sqrMagnitudePrecision;  // TODO: inline vector methods to optimize?
        }
    }
}