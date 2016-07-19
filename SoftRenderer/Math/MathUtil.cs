using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SoftRenderer.Math
{
    class MathUtil
    {
        // 当使用Math类的三角函数的时候，所有的单位都是用弧度表示的
        public static float ConvertDegreesToRadians(float degree){
            float radians = (float)(System.Math.PI / 180) * degree;
            return radians;
        }

        public static float Lerp(float a, float b, float t)
        {
            t = System.Math.Max(t, 0);
            t = System.Math.Min(t, 1);
            return b * t + (1 - t) * a;

        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            Vector2 res = new Vector2();
            res.x = MathUtil.Lerp(a.x, b.x, t);
            res.y = MathUtil.Lerp(a.y, b.y, t);
            return res;
        }

        public static int Clamp(int v, int min, int max)
        {
            v = System.Math.Max(min, v);
            v = System.Math.Min(v, max);
            return v;
        }

        public static float Clamp(float v, float min, float max)
        {
            v = System.Math.Max(min, v);
            v = System.Math.Min(v, max);
            return v;
        }
        
        
    }
}
