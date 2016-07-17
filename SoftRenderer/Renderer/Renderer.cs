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

        public void Draw(Vertex[] vertices, int[] triangles, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p, Material material)
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

                    DrawTrangle(vertex0, vertex1, vertex2, m, v, p, material);
                }
            }
            
        }

        private bool BackFaceCulling(Vertex p1, Vertex p2, Vertex p3)
        {
            Vector4 v1 = p2.pos - p1.pos;
            Vector4 v2 = p3.pos - p2.pos;
            Vector4 normal = Vector4.Cross(v1, v2);
            //由于在视空间中，所以相机点就是（0,0,0）
            Vector4 viewDir = p1.pos - new Vector4(0, 0, 0, 1);
            if (Vector4.Dot(normal, viewDir) < 0)
            {
                return true;
            }
            return false;
            
        }


        private bool Clip(Vertex v)
        {
            if (v.pos.x >= -v.pos.w && v.pos.x <= v.pos.w &&
                v.pos.y >= -v.pos.w && v.pos.y <= v.pos.w &&
                v.pos.z >= -v.pos.w && v.pos.z <= v.pos.w)
            {
                return true;
            }
            return false;
        }

        private void PerspectiveDivision( Vertex v)
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

        public void DrawTrangle(Vertex vertex0, Vertex vertex1, Vertex vertex2, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p, Material material)
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
            if (BackFaceCulling(vertex0, vertex1, vertex2))
            {
                return;
            }


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
            PerspectiveDivision( vertex0);
            PerspectiveDivision( vertex1);
            PerspectiveDivision( vertex2);

            // cvv到屏幕坐标
            TransformToScreen( vertex0);
            TransformToScreen( vertex1);
            TransformToScreen( vertex2);


            //渲染
            if (_renderMode == RenderMode.Wireframe)
            {
                BresenhamDrawLine(vertex0, vertex1);
                BresenhamDrawLine(vertex1, vertex2);
                BresenhamDrawLine(vertex2, vertex0);
            }
            else if(_renderMode == RenderMode.Textured)
            {

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

        /*
        private void TriangleRasterization(Vertex p1, Vertex p2, Vertex p3)
        {
            if (p1.pos.y == p2.pos.y)
            {
                if (p1.pos.y < p3.pos.y)
                {//平顶
                    DrawTriangleTop(p1, p2, p3);
                }
                else
                {//平底
                    DrawTriangleBottom(p3, p1, p2);
                }
            }
            else if (p1.pos.y == p3.pos.y)
            {
                if (p1.pos.y < p2.pos.y)
                {//平顶
                    DrawTriangleTop(p1, p3, p2);
                }
                else
                {//平底
                    DrawTriangleBottom(p2, p1, p3);
                }
            }
            else if (p2.pos.y == p3.pos.y)
            {
                if (p2.pos.y < p1.pos.y)
                {//平顶
                    DrawTriangleTop(p2, p3, p1);
                }
                else
                {//平底
                    DrawTriangleBottom(p1, p2, p3);
                }
            }
            else
            {//分割三角形
                Vertex top;

                Vertex bottom;
                Vertex middle;
                if (p1.pos.y > p2.pos.y && p2.pos.y > p3.pos.y)
                {
                    top = p3;
                    middle = p2;
                    bottom = p1;
                }
                else if (p3.pos.y > p2.pos.y && p2.pos.y > p1.pos.y)
                {
                    top = p1;
                    middle = p2;
                    bottom = p3;
                }
                else if (p2.pos.y > p1.pos.y && p1.pos.y > p3.pos.y)
                {
                    top = p3;
                    middle = p1;
                    bottom = p2;
                }
                else if (p3.pos.y > p1.pos.y && p1.pos.y > p2.pos.y)
                {
                    top = p2;
                    middle = p1;
                    bottom = p3;
                }
                else if (p1.pos.y > p3.pos.y && p3.pos.y > p2.pos.y)
                {
                    top = p2;
                    middle = p3;
                    bottom = p1;
                }
                else if (p2.pos.y > p3.pos.y && p3.pos.y > p1.pos.y)
                {
                    top = p1;
                    middle = p3;
                    bottom = p2;
                }
                else
                {
                    //三点共线
                    return;
                }
                //插值求中间点x
                float middlex = (middle.pos.y - top.pos.y) * (bottom.pos.x - top.pos.x) / (bottom.pos.y - top.pos.y) + top.pos.x;

                float dy = middle.pos.y - top.pos.y;
                float t = dy / (bottom.pos.y - top.pos.y);
                //插值生成左右顶点
                // 生成新的middle顶点
                Vertex newMiddle = new Vertex();
                newMiddle.pos.x = middlex;
                newMiddle.pos.y = middle.pos.y;
                //MathUntil.ScreenSpaceLerpVertex(ref newMiddle, top, bottom, t);
                // 插值生成


                //平底
                DrawTriangleBottom(top, newMiddle, middle);
                //平顶
                DrawTriangleTop(newMiddle, middle, bottom);
            }
        }

        

        private void DrawTriangleTop(Vertex p1, Vertex p2, Vertex p3)
        {
            for (float y = p1.pos.y; y <= p3.pos.y; y += 0.5f)
            {
                int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
                if (yIndex >= 0 && yIndex < this.MaximumSize.Height)
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

                    // 线性插值其他的属性，例如uv，lightColor
                    // t 是 屏幕的 x'或者是y'的比例,就是三角形的pos映射到屏幕的pos'的x',y'
                    MathUntil.ScreenSpaceLerpVertex(ref new1, p1, p3, t);
                    //
                    Vertex new2 = new Vertex();
                    new2.pos.x = xr;
                    new2.pos.y = y;
                    MathUntil.ScreenSpaceLerpVertex(ref new2, p2, p3, t);
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
                if (yIndex >= 0 && yIndex < this.MaximumSize.Height)
                {
                    float xl = (y - p1.pos.y) * (p2.pos.x - p1.pos.x) / (p2.pos.y - p1.pos.y) + p1.pos.x;
                    float xr = (y - p1.pos.y) * (p3.pos.x - p1.pos.x) / (p3.pos.y - p1.pos.y) + p1.pos.x;

                    float dy = y - p1.pos.y;
                    float t = dy / (p2.pos.y - p1.pos.y);
                    //插值生成左右顶点
                    Vertex new1 = new Vertex();
                    new1.pos.x = xl;
                    new1.pos.y = y;
                    // t 是 屏幕的 x'或者是y'的比例
                    MathUntil.ScreenSpaceLerpVertex(ref new1, p1, p2, t);
                    //
                    Vertex new2 = new Vertex();
                    new2.pos.x = xr;
                    new2.pos.y = y;
                    MathUntil.ScreenSpaceLerpVertex(ref new2, p1, p3, t);
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
                if (xIndex >= 0 && xIndex < this.MaximumSize.Width)
                {
                    float lerpFactor = 0;
                    if (dx != 0)
                    {
                        // 这个也是pos映射到屏幕上的pos',的x',y'的线性插值
                        lerpFactor = (x - left.pos.x) / dx;
                    }
                    //1/z’与x’和y'是线性关系的
                    float onePreZ = MathUntil.Lerp(left.onePerZ, right.onePerZ, lerpFactor);
                    if (onePreZ >= _zBuff[yIndex, xIndex])//使用1/z进行深度测试
                    {//通过测试
                        // 原来的z
                        float w = 1 / onePreZ;
                        _zBuff[yIndex, xIndex] = onePreZ;
                        //uv 插值，求纹理颜色，s/z 与x' 成正比
                        float u = MathUntil.Lerp(left.u, right.u, lerpFactor) * w * (_texture.Width - 1);
                        float v = MathUntil.Lerp(left.v, right.v, lerpFactor) * w * (_texture.Height - 1);

                        //纹理采样
                        SoftRenderer.RenderData.Color texColor = new RenderData.Color(1, 1, 1);

                        //点采样
                        if (_textureFilterMode == TextureFilterMode.point)
                        {
                            int uIndex = (int)System.Math.Round(u, MidpointRounding.AwayFromZero);
                            int vIndex = (int)System.Math.Round(v, MidpointRounding.AwayFromZero);
                            uIndex = MathUntil.Range(uIndex, 0, _texture.Width - 1);
                            vIndex = MathUntil.Range(vIndex, 0, _texture.Height - 1);
                            //uv坐标系采用dx风格
                            texColor = new RenderData.Color(ReadTexture(uIndex, vIndex));//转到我们自定义的color进行计算
                        }
                        //双线性采样
                        else if (_textureFilterMode == TextureFilterMode.Bilinear)
                        {
                            // 小于或等于u的整数
                            float uIndex = (float)System.Math.Floor(u);
                            float vIndex = (float)System.Math.Floor(v);
                            float du = u - uIndex;
                            float dv = v - vIndex;

                            // 双线性插值
                            SoftRenderer.RenderData.Color texcolor1 = new RenderData.Color(ReadTexture((int)uIndex, (int)vIndex)) * (1 - du) * (1 - dv);
                            SoftRenderer.RenderData.Color texcolor2 = new RenderData.Color(ReadTexture((int)uIndex + 1, (int)vIndex)) * du * (1 - dv);
                            SoftRenderer.RenderData.Color texcolor3 = new RenderData.Color(ReadTexture((int)uIndex, (int)vIndex + 1)) * (1 - du) * dv;
                            SoftRenderer.RenderData.Color texcolor4 = new RenderData.Color(ReadTexture((int)uIndex + 1, (int)vIndex + 1)) * du * dv;
                            texColor = texcolor1 + texcolor2 + texcolor3 + texcolor4;
                        }

                        //插值顶点颜色
                        SoftRenderer.RenderData.Color vertColor = MathUntil.Lerp(left.vcolor, right.vcolor, lerpFactor) * w;
                        //插值光照颜色
                        SoftRenderer.RenderData.Color lightColor = MathUntil.Lerp(left.lightingColor, right.lightingColor, lerpFactor) * w; ;


                        if (_lightMode == LightMode.On)
                        {//光照模式，需要混合光照的颜色
                            if (RenderMode.Textured == _currentMode)
                            {
                                SoftRenderer.RenderData.Color finalColor = texColor * lightColor;
                                _frameBuff.SetPixel(xIndex, yIndex, finalColor.TransFormToSystemColor());
                            }
                            else if (RenderMode.VertexColor == _currentMode)
                            {
                                SoftRenderer.RenderData.Color finalColor = vertColor * lightColor;
                                _frameBuff.SetPixel(xIndex, yIndex, finalColor.TransFormToSystemColor());
                            }
                        }
                        else
                        {
                            if (RenderMode.Textured == _currentMode)
                            {
                                _frameBuff.SetPixel(xIndex, yIndex, texColor.TransFormToSystemColor());
                            }
                            else if (RenderMode.VertexColor == _currentMode)
                            {
                                _frameBuff.SetPixel(xIndex, yIndex, vertColor.TransFormToSystemColor());
                            }
                        }
                    }
                }
            }

        }
         * */
    }
}
