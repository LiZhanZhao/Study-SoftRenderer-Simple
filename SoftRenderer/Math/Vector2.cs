using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderer.Math
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator *(Vector2 lhs, float rhs)
        {
            Vector2 v = new Vector2();
            v.x = lhs.x * rhs;
            v.y = lhs.y * rhs;
            return v;
        }

        public static Vector2 operator /(Vector2 lhs, float rhs)
        {
            Vector2 v = new Vector2();
            v.x = lhs.x / rhs;
            v.y = lhs.y / rhs;
            return v;
        }
    }
}
