using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;
namespace SoftRenderer.Renderer
{
    class Util
    {
        public static int screenWidth;
        public static int screenHeight;
        public static void TransitionVertexList(ref Vertex[] vertices, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
        {
            for (int i = 0; i < vertices.Length / 3; i++)
            {
                TransitionTrangle(ref vertices[i * 3], ref vertices[i * 3 + 1], ref vertices[i * 3 + 2], m, v, p);
            }
        }


        public static void TransitionTrangle(ref Vertex vertex0, ref Vertex vertex1, ref Vertex vertex2, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
        {
            // 注意，变换完了，那么顶点的数据就已经变了，试试struct
            // 先变换
            // 世界变换
            vertex0.pos = m * vertex0.pos;
            vertex1.pos = m * vertex1.pos;
            vertex2.pos = m * vertex2.pos;

            // 相机变换
            vertex0.pos = v * vertex0.pos;
            vertex1.pos = v * vertex1.pos;
            vertex2.pos = v * vertex2.pos;

            //在相机空间进行背面消隐
            //if (BackFaceCulling(vertex0, vertex1, vertex2))
            //{
            //    return;
            //}


            // 投影变换,cvv : x,y,z : [-w, w]
            vertex0.pos = p * vertex0.pos;
            vertex1.pos = p * vertex1.pos;
            vertex2.pos = p * vertex2.pos;

            //进行裁剪
            //if (Clip(vertex0) == false || Clip(vertex1) == false || Clip(vertex2) == false)
            //{
            //    return;
            //}

            // 透视除法, cvv : x,y,z : [-1,1]
            PerspectiveDivision(ref vertex0);
            PerspectiveDivision(ref vertex1);
            PerspectiveDivision(ref vertex2);

            // cvv到屏幕坐标
            TransformToScreen(ref vertex0);
            TransformToScreen(ref vertex1);
            TransformToScreen(ref vertex2);



        }

        private static void PerspectiveDivision(ref Vertex v)
        {
            if (v.pos.w != 0)
            {
                v.pos.x *= 1 / v.pos.w;
                v.pos.y *= 1 / v.pos.w;
                v.pos.z *= 1 / v.pos.w;
                v.pos.w = 1;
            }
        }

        private static void TransformToScreen(ref Vertex v)
        {
            v.pos.x = (v.pos.x + 1) * 0.5f * screenWidth;
            v.pos.y = (1 - v.pos.y) * 0.5f * screenHeight;
        }
    }
}
