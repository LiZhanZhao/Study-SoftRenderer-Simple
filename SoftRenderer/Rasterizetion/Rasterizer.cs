using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Renderer;

namespace SoftRenderer.Rasterizetion
{
    public enum PrimitiveMode
    {
        Lines,
        Triangles,
        Triangle_Strip,

    }
    // 光栅化类
    public class Rasterizer
    {
        private static Rasterizer _instance = null;
        private RenderTarget _renderTarget = null;
        public static Rasterizer Instance()
        {
            if (_instance == null)
            {
                _instance = new Rasterizer();
            }
            return _instance;
        }

        public void Render(PrimitiveMode pm, Vertex[] vertexList)
        {
            if (pm == PrimitiveMode.Lines)
            {
                this.RenderLines(vertexList);
            }
            else if (pm == PrimitiveMode.Triangles)
            {
                this.RenderTriangles(vertexList);
            }
            
        }

        void RenderTriangles(Vertex[] vertex)
        {
            for (int i = 0; i < vertex.Length / 3; i++)
            {
                Vertex vertex0 = vertex[i * 3];
                Vertex vertex1 = vertex[i * 3 + 1];
                Vertex vertex2 = vertex[i * 3 + 2];

                BresenhamDrawLine(vertex0, vertex1);
                BresenhamDrawLine(vertex1, vertex2);
                BresenhamDrawLine(vertex2, vertex0);
            }
        }

        void RenderLines(Vertex[] vertex)
        {
            for (int i = 0; i < vertex.Length / 2; i++)
            {
                Vertex v0 = vertex[i * 2];
                Vertex v1 = vertex[i * 2 + 1];
                BresenhamDrawLine(v0, v1);
            }
        }

        public void SetRenderTarget(RenderTarget target)
        {
            _renderTarget = target;
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
                    //_canvasBuff.SetPixel(x, y, System.Drawing.Color.White);
                    if (_renderTarget != null)
                    {
                        _renderTarget.Write(x, y, System.Drawing.Color.White);
                    }

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
                    //_canvasBuff.SetPixel(x, y, System.Drawing.Color.White);
                    if (_renderTarget != null)
                    {
                        _renderTarget.Write(x, y, System.Drawing.Color.White);
                    }

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
