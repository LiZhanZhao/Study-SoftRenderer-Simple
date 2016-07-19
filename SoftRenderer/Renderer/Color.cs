using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;
namespace SoftRenderer.Renderer
{
    //[0-1] 的颜色
    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Color(System.Drawing.Color c)
        {
            this.r = MathUtil.Clamp((float)c.R / 255, 0, 1);
            this.g = MathUtil.Clamp((float)c.G / 255, 0, 1);
            this.b = MathUtil.Clamp((float)c.B / 255, 0, 1);
            this.a = MathUtil.Clamp((float)c.A / 255, 0, 1);
        }

        public static Color operator *(Color a, float b)
        {
            Color c = new Color();
            c.r = a.r * b;
            c.g = a.g * b;
            c.b = a.b * b;
            c.a = a.a * b;
            return c;
        }

        public static Color operator +(Color a, Color b)
        {
            Color c = new Color();
            c.r = a.r + b.r;
            c.g = a.g + b.g;
            c.b = a.b + b.b;
            c.a = a.a + b.a;
            return c;
        }

        public System.Drawing.Color TransFormToSystemColor()
        {
            float r = this.r * 255;
            float g = this.g * 255;
            float b = this.b * 255;
            float a = this.a * 255;
            return System.Drawing.Color.FromArgb((int)a, (int)r, (int)g, (int)b);
            //return System.Drawing.Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
