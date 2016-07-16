using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using SoftRenderer.Forms;

namespace SoftRenderer
{
    // 说明：
    // 1. 矩阵，OpenGL，列为主，矩阵在左，向量在右
    // 2. 右手坐标系，负Z轴为camera的观察方向
    static class Program
    {
        // 这里要先更改项目的输出类型为Windows Application.这样就不会出来Dos Window
        static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }
}
