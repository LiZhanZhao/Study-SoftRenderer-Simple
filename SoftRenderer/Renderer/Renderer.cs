using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using SoftRenderer.Math;
namespace SoftRenderer.Renderer
{
    public enum RenderMode
    {
        /// <summary>
        /// 线框模式
        /// </summary>
        Wireframe,
        /// <summary>
        /// 纹理
        /// </summary>
        Textured,
        /// <summary>
        /// 顶点色
        /// </summary>
        VertexColor
    }

    class Renderer
    {
        private static Renderer _instance = null;
        public static Renderer Instance()
        {
            if (_instance == null)
            {
                _instance = new Renderer();
            }
            return _instance;
        }

        private RenderMode _renderMode = RenderMode.Textured;
        public void SetRenderMode(RenderMode mode)
        {
            _renderMode = mode;
        }

        private Bitmap _canvasBuff = null;
        public void SetCanvasBuff(Bitmap buff)
        {
            _canvasBuff = buff;
        }

        private int _screenWidth, _screenHeight;
        public void SetScreenWidthHeight(int w, int h)
        {
            _screenWidth = w;
            _screenHeight = h;
        }

        public void Draw(Vertex[] vertices, int[] triangles, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
        {
            if (_canvasBuff != null)
            {
                for (int tri = 0; tri < triangles.Length / 3; tri++)
                {
                    int triIndex0 = triangles[tri * 3];
                    int triIndex1 = triangles[tri * 3 + 1];
                    int triIndex2 = triangles[tri * 3 + 2];

                    Vertex vertex0 = vertices[triIndex0];
                    Vertex vertex1 = vertices[triIndex1];
                    Vertex vertex2 = vertices[triIndex2];

                    DrawTrangle(vertex0, vertex1, vertex2, m, v, p);
                }

                


            }
            
        }

        // 因为是类似openGl的方式，所以，x,y,z都是[-1,1]
        private bool Clip(Vertex v)
        {
            //open cvv为 x:-1,1  y:-1,1  z:-1,1
            if (v.pos.x >= -v.pos.w && v.pos.x <= v.pos.w &&
                v.pos.y >= -v.pos.w && v.pos.y <= v.pos.w &&
                v.pos.z >= -v.pos.w && v.pos.z <= v.pos.w)
            {
                return true;
            }
            return false;
        }

        private void PerspectiveDivision(Vertex v)
        {
            if (v.pos.w != 0)
            {
                v.pos.x *= 1 / v.pos.w;
                v.pos.y *= 1 / v.pos.w;
                v.pos.z *= 1 / v.pos.w;
                v.pos.w = 1;
            }
        }

        private void TransformToScreen(Vertex v)
        {
            v.pos.x = (v.pos.x + 1) * 0.5f * this._screenWidth;
            v.pos.y = (1 - v.pos.y) * 0.5f * this._screenHeight;
        }

        public void DrawTrangle(Vertex vertex0, Vertex vertex1, Vertex vertex2, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p)
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
            // 暂定


            // 投影变换,cvv : x,y,z : [-w, w]
            vertex0.pos = p * vertex0.pos;
            vertex1.pos = p * vertex1.pos;
            vertex2.pos = p * vertex2.pos;

            //进行裁剪
            if (Clip(vertex0) == false || Clip(vertex1) == false || Clip(vertex2) == false)
            {
                return;
            }

            // 透视除法, cvv : x,y,z : [-1,1]
            PerspectiveDivision(vertex0);
            PerspectiveDivision(vertex1);
            PerspectiveDivision(vertex2);

            // cvv到屏幕坐标
            TransformToScreen(vertex0);
            TransformToScreen(vertex1);
            TransformToScreen(vertex2);


            //再渲染
            if (_renderMode == RenderMode.Wireframe)
            {
                BresenhamDrawLine(vertex0, vertex1);
                BresenhamDrawLine(vertex1, vertex2);
                BresenhamDrawLine(vertex2, vertex0);
            }
            

        }

        private void BresenhamDrawLine(Vertex p1, Vertex p2)
        {
            int x = (int)(System.Math.Round(p1.pos.x, MidpointRounding.AwayFromZero));
            int y = (int)(System.Math.Round(p1.pos.y, MidpointRounding.AwayFromZero));
            int dx = (int)(System.Math.Round(p2.pos.x - p1.pos.x, MidpointRounding.AwayFromZero));
            int dy = (int)(System.Math.Round(p2.pos.y - p1.pos.y, MidpointRounding.AwayFromZero));
            int stepx = 1;
            int stepy = 1;

            if (dx >= 0)
            {
                stepx = 1;
            }
            else
            {
                stepx = -1;
                dx = System.Math.Abs(dx);
            }

            if (dy >= 0)
            {
                stepy = 1;
            }
            else
            {
                stepy = -1;
                dy = System.Math.Abs(dy);
            }

            int dx2 = 2 * dx;
            int dy2 = 2 * dy;

            if (dx > dy)
            {
                int error = dy2 - dx;
                for (int i = 0; i <= dx; i++)
                {
                    _canvasBuff.SetPixel(x, y, System.Drawing.Color.White);
                    if (error >= 0)
                    {
                        error -= dx2;
                        y += stepy;
                    }
                    error += dy2;
                    x += stepx;

                }
            }
            else
            {
                int error = dx2 - dy;
                for (int i = 0; i <= dy; i++)
                {
                    _canvasBuff.SetPixel(x, y, System.Drawing.Color.White);
                    if (error >= 0)
                    {
                        error -= dy2;
                        x += stepx;
                    }
                    error += dx2;
                    y += stepy;

                }
            }

        }
    }
}
