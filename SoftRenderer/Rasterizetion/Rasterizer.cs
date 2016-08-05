using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Renderer;
using SoftRenderer.Math;
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

        //private int _screenWidth, _screenHeight;
        //public void SetScreenWidthHeight(int w, int h)
        //{
        //    _screenWidth = w;
        //    _screenHeight = h;
        //}

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

                TriangleRasterization(vertex0, vertex1, vertex2);
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
                    if (_renderTarget != null && dx != 0)
                    {
                        float t = i / (float)dx;
                        Color resCol = MathUtil.Lerp(p1.color, p2.color, t);
                        _renderTarget.Write(x, y, resCol.TransFormToSystemColor());
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
                    if (_renderTarget != null && dy != 0)
                    {
                        float t = i / (float)dy;
                        Color resCol = MathUtil.Lerp(p1.color, p2.color, t);
                        _renderTarget.Write(x, y, resCol.TransFormToSystemColor());
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

        private void GetTopMiddleBottom(Vertex[] vertexList, ref Vertex top, ref Vertex middle, ref Vertex bottom)
        {
            if (vertexList != null && vertexList.Length == 3)
            {
                float minY = vertexList[0].pos.y;
                int minIndex = 0;
                float maxY = vertexList[0].pos.y;
                int maxIndex = 0;
                for (int i = 0; i < vertexList.Length; i++)
                {
                    Vector4 pos = vertexList[i].pos;
                    if (minY > pos.y)
                    {
                        minY = pos.y;
                        minIndex = i;
                    }

                    if (maxY < pos.y)
                    {
                        maxY = pos.y;
                        maxIndex = i;
                    }
                }

                int middleIndex = 0;
                for (int i = 0; i < vertexList.Length; i++)
                {
                    if (i != minIndex && i != maxIndex)
                    {
                        middleIndex = i;
                        break;
                    }
                }

                top = vertexList[minIndex];
                bottom = vertexList[maxIndex];
                middle = vertexList[middleIndex];
                
            }
            

        }
        private void TriangleRasterization(Vertex p1, Vertex p2, Vertex p3)
        {
            GetTopMiddleBottom(new Vertex[] { p1, p2, p3 }, ref p1, ref p2, ref p3);
            Vertex top = p1;
            Vertex middle = p2;
            Vertex bottom = p3;

            if (top.pos.y == middle.pos.y)
            {
                DrawTriangleTop(top, middle, bottom);
            }
            else if (middle.pos.y == bottom.pos.y)
            {
                DrawTriangleBottom(top, middle, bottom);
            }
            else
            {
                if (top.pos.y == middle.pos.y && middle.pos.y == bottom.pos.y)
                {
                    return;
                }
                else
                {
                    //插值求中间点x
                    float middlex = (middle.pos.y - top.pos.y) * (bottom.pos.x - top.pos.x) / (bottom.pos.y - top.pos.y) + top.pos.x;

                    float dy = middle.pos.y - top.pos.y;
                    float t = dy / (bottom.pos.y - top.pos.y);
                    //插值生成左右顶点
                    // 生成新的middle顶点
                    Vertex newMiddle = new Vertex();

                    newMiddle.pos.x = middlex;
                    newMiddle.pos.y = middle.pos.y;

                    // 插值生成
                    ScreenSpaceLerpVertex(ref newMiddle, top, bottom, t);

                    //平顶
                    DrawTriangleTop(newMiddle, middle, bottom);
                    //平底
                    DrawTriangleBottom(top, newMiddle, middle);
                }
            }
        }

        private void DrawTriangleTop(Vertex p1, Vertex p2, Vertex p3)
        {
            for (float y = p1.pos.y; y <= p3.pos.y; y += 0.5f)
            {
                int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
                if (yIndex >= 0 && yIndex < this._renderTarget.Height())
                {
                    // 公式
                    float xl = (y - p1.pos.y) * (p3.pos.x - p1.pos.x) / (p3.pos.y - p1.pos.y) + p1.pos.x;
                    float xr = (y - p2.pos.y) * (p3.pos.x - p2.pos.x) / (p3.pos.y - p2.pos.y) + p2.pos.x;

                    float dy = y - p1.pos.y;

                    float t = dy / (p3.pos.y - p1.pos.y);

                    //插值生成左右顶点
                    Vertex new1 = new Vertex();
                    new1.pos.x = xl;
                    new1.pos.y = y;
                    // t 是 屏幕的 x'或者是y'的比例,就是三角形的pos映射到屏幕的pos'的x',y'
                    ScreenSpaceLerpVertex(ref new1, p1, p3, t);

                    Vertex new2 = new Vertex();
                    new2.pos.x = xr;
                    new2.pos.y = y;
                    ScreenSpaceLerpVertex(ref new2, p2, p3, t);

                    //扫描线填充
                    if (new1.pos.x < new2.pos.x)
                    {
                        ScanlineFill(new1, new2, yIndex);
                    }
                    else
                    {
                        ScanlineFill(new2, new1, yIndex);
                    }
                }
            }
        }

        private void DrawTriangleBottom(Vertex p1, Vertex p2, Vertex p3)
        {
            for (float y = p1.pos.y; y <= p2.pos.y; y += 0.5f)
            {
                int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
                if (yIndex >= 0 && yIndex < this._renderTarget.Height())
                {
                    float xl = (y - p1.pos.y) * (p2.pos.x - p1.pos.x) / (p2.pos.y - p1.pos.y) + p1.pos.x;
                    float xr = (y - p1.pos.y) * (p3.pos.x - p1.pos.x) / (p3.pos.y - p1.pos.y) + p1.pos.x;

                    float dy = y - p1.pos.y;
                    float t = dy / (p2.pos.y - p1.pos.y);
                    //插值生成左右顶点
                    Vertex new1 = new Vertex();
                    new1.pos.x = xl;
                    new1.pos.y = y;

                    ScreenSpaceLerpVertex(ref new1, p1, p2, t);

                    Vertex new2 = new Vertex();
                    new2.pos.x = xr;
                    new2.pos.y = y;
                    ScreenSpaceLerpVertex(ref new2, p1, p3, t);
                    //扫描线填充
                    if (new1.pos.x < new2.pos.x)
                    {
                        ScanlineFill(new1, new2, yIndex);
                    }
                    else
                    {
                        ScanlineFill(new2, new1, yIndex);
                    }
                }
            }
        }

        // 变换到屏幕的x',y'，与自身的1/z成正比，与自身的u/z，v/z成正比，可以证明
        // 例如: x1'和x2’之间的xm',
        // 线性关系 : xm' = x1' * t + (1- t) * x2'

        // xm'的1/zm 与1/z1,1/z2成正比，
        // 线性关系 : 1/zm = 1/z1 * t + (1- t) * 1/z2

        // xm'的um/zm 与 u1/z1, u2/z2成正比
        // 线性关系 : um/zm = u1/z1 * t + (1- t) * u2/z2

        // 需要注意的就是，t是屏幕的x1'和x2'坐标的比值
        public static void ScreenSpaceLerpVertex(ref Vertex v, Vertex v1, Vertex v2, float t)
        {
            float onePreZ_Top = 1 / v1.pos.z;
            float onePreZ_Bottom = 1 / v2.pos.z;
            float onePreZ_Middle = MathUtil.Lerp(onePreZ_Top, onePreZ_Bottom, t);
            // z
            v.pos.z = 1 / onePreZ_Middle;

            Vector2 uvPreZ_Top = new Vector2(v1.uv.x * onePreZ_Top, v1.uv.y * onePreZ_Top);
            Vector2 uvPreZ_Bottom = new Vector2(v2.uv.x * onePreZ_Bottom, v2.uv.y * onePreZ_Bottom);
            Vector2 uvPreZ_Middle = MathUtil.Lerp(uvPreZ_Top, uvPreZ_Bottom, t);
            Vector2 uv = uvPreZ_Middle / onePreZ_Middle;
            // uv
            v.uv = uv;

            // color
            v.color = MathUtil.Lerp(v1.color, v2.color, t);

        }

        private void ScanlineFill(Vertex left, Vertex right, int yIndex)
        {
            float dx = right.pos.x - left.pos.x;
            float step = 1;
            if (dx != 0)
            {
                step = 1 / dx;
            }
            for (float x = left.pos.x; x <= right.pos.x; x += 0.5f)
            {
                int xIndex = (int)(x + 0.5f);
                if (xIndex >= 0 && xIndex < this._renderTarget.Width())
                {
                    float lerpFactor = 0;
                    if (dx != 0)
                    {
                        // 这个也是pos映射到屏幕上的pos',的x',y'的线性插值
                        lerpFactor = (x - left.pos.x) / dx;

                        // 颜色插值
                        if (_renderTarget != null)
                        {
                            Color resCol = MathUtil.Lerp(left.color, right.color, lerpFactor);
                            _renderTarget.Write(xIndex, yIndex, resCol.TransFormToSystemColor());
                        }
                    }

                    

                    /*
                    //1/z’与x’和y'是线性关系的
                    //float onePreZ_Left = 1 / left.pos.z;
                    //float onePreZ_Right = 1 / right.pos.z;
                    //float onePreZ = MathUtil.Lerp(onePreZ_Left, onePreZ_Right, lerpFactor);

                    
                    //使用1/z进行深度测试
                    //通过测试
                    if (onePreZ >= _zBuff[yIndex, xIndex])
                    {

                        //_zBuff[yIndex, xIndex] = onePreZ;
                        //Bitmap mainTexture = material.GetMainTexture();
                        ////uv 插值，求纹理颜色，s/z 与x' 成正比
                        //float u = MathUtil.Lerp(left.uv.x / left.pos.z, right.uv.x / right.pos.z, lerpFactor) / onePreZ * (mainTexture.Width - 1);
                        //float v = MathUtil.Lerp(left.uv.y / left.pos.z, right.uv.y / right.pos.z, lerpFactor) / onePreZ * (mainTexture.Height - 1);

                        ////纹理采样
                        //Color texColor = new Color(1, 1, 1, 1);

                        ////点采样
                        //if (_textureFilterMode == TextureFilterMode.point)
                        //{
                        //    int uIndex = (int)System.Math.Round(u, MidpointRounding.AwayFromZero);
                        //    int vIndex = (int)System.Math.Round(v, MidpointRounding.AwayFromZero);
                        //    uIndex = MathUtil.Clamp(uIndex, 0, mainTexture.Width - 1);
                        //    vIndex = MathUtil.Clamp(vIndex, 0, mainTexture.Height - 1);
                        //    //转到我们自定义的color进行计算
                        //    System.Drawing.Color originCol = ReadTexture(uIndex, vIndex, mainTexture);
                        //    texColor = new Color(originCol);
                        //}
                        ////双线性采样
                        //else if (_textureFilterMode == TextureFilterMode.Bilinear)
                        //{
                        //    // 小于或等于u的整数
                        //    float uIndex = (float)System.Math.Floor(u);
                        //    float vIndex = (float)System.Math.Floor(v);
                        //    float du = u - uIndex;
                        //    float dv = v - vIndex;

                        //    // 双线性插值
                        //    Color texcolor1 = new Color(ReadTexture((int)uIndex, (int)vIndex, mainTexture)) * (1 - du) * (1 - dv);
                        //    Color texcolor2 = new Color(ReadTexture((int)uIndex + 1, (int)vIndex, mainTexture)) * du * (1 - dv);
                        //    Color texcolor3 = new Color(ReadTexture((int)uIndex, (int)vIndex + 1, mainTexture)) * (1 - du) * dv;
                        //    Color texcolor4 = new Color(ReadTexture((int)uIndex + 1, (int)vIndex + 1, mainTexture)) * du * dv;
                        //    texColor = texcolor1 + texcolor2 + texcolor3 + texcolor4;
                        //}

                        //if (_renderMode == RenderMode.Textured)
                        //{
                        //    _canvasBuff.SetPixel(xIndex, yIndex, texColor.TransFormToSystemColor());
                        //}

                        //插值顶点颜色
                        //SoftRenderer.RenderData.Color vertColor = MathUntil.Lerp(left.vcolor, right.vcolor, lerpFactor) * w;
                        //插值光照颜色
                        //SoftRenderer.RenderData.Color lightColor = MathUntil.Lerp(left.lightingColor, right.lightingColor, lerpFactor) * w; ;  
                    }
                    */
                }
            }

        }
    }
}
