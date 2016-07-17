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
            t = System.Math.Min(t, 0);
            t = System.Math.Max(t, 1);
            return b * t + (1 - t) * a;

        }

        
    }
}
