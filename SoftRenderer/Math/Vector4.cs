using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderer.Math
{
    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;


        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float Length
        {
            get
            {
                float sq = x * x + y * y + z * z;
                return (float)System.Math.Sqrt(sq);
            }
        }

        public void Normalize()
        {
            float len = Length;
            if (len != 0)
            {
                float s = 1 / len;
                x *= s;
                y *= s;
                z *= s;
            }
        }

        public static Vector4 operator -(Vector4 lhs, Vector4 rhs)
        {
            Vector4 v = new Vector4();
            v.x = lhs.x - rhs.x;
            v.y = lhs.y - rhs.y;
            v.z = lhs.z - rhs.z;
            // 其实这里可以直接写0,因为向量相减，w是0，点相减变成向量，w也是0
            v.w = lhs.w - rhs.w;
            return v;
        }

        public static Vector4 Cross(Vector4 lhs, Vector4 rhs)
        {
            float x = lhs.y * rhs.z - lhs.z * rhs.y;
            float y = lhs.z * rhs.x - lhs.x * rhs.z;
            float z = lhs.x * rhs.y - lhs.y * rhs.x;
            return new Vector4(x, y, z, 0);
        }

        public static float Dot(Vector4 lhs, Vector4 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }
    }
}
