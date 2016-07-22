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

    /// <summary>
    /// 纹理过滤模式
    /// </summary>
    public enum TextureFilterMode
    {
        /// <summary>
        /// 点采样
        /// </summary>
        point,
        /// <summary>
        /// 双线性采样
        /// </summary>
        Bilinear
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

        private float[,] _zBuff;
        public void SetZBuff(float[,] buff)
        {
            _zBuff = buff;
        }

        
        private TextureFilterMode _textureFilterMode = TextureFilterMode.point;
        public void SetTextureFilterMode(TextureFilterMode mode)
        {
            _textureFilterMode = mode;
        }

        

        public void Draw(Vertex[] vertices, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p, Material material)
        {
            if (_canvasBuff != null)
            {
                for (int i = 0; i < vertices.Length / 3; i++)
                {
                    Vertex vertex0 = vertices[i * 3];
                    Vertex vertex1 = vertices[i * 3 + 1];
                    Vertex vertex2 = vertices[i * 3 + 2];

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

        private void PerspectiveDivision(ref Vertex v)
        {
            if (v.pos.w != 0)
            {
                v.pos.x *= 1 / v.pos.w;
                v.pos.y *= 1 / v.pos.w;
                v.pos.z *= 1 / v.pos.w;
                v.pos.w = 1;
            }
        }

        private void TransformToScreen(ref Vertex v)
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
            PerspectiveDivision(ref vertex0);
            PerspectiveDivision(ref vertex1);
            PerspectiveDivision(ref vertex2);

            // cvv到屏幕坐标
            TransformToScreen(ref vertex0);
            TransformToScreen(ref vertex1);
            TransformToScreen(ref vertex2);


            //渲染
            if (_renderMode == RenderMode.Wireframe)
            {
                //BresenhamDrawLine(vertex0, vertex1);
                //BresenhamDrawLine(vertex1, vertex2);
                //BresenhamDrawLine(vertex2, vertex0);
            }
            else if(_renderMode == RenderMode.Textured)
            {
                TriangleRasterization(vertex0, vertex1, vertex2, material);
            }
            

        }

        //private void BresenhamDrawLine(Vertex p1, Vertex p2)
        //{
        //    int x = (int)(System.Math.Round(p1.pos.x, MidpointRounding.AwayFromZero));
        //    int y = (int)(System.Math.Round(p1.pos.y, MidpointRounding.AwayFromZero));
        //    int dx = (int)(System.Math.Round(p2.pos.x - p1.pos.x, MidpointRounding.AwayFromZero));
        //    int dy = (int)(System.Math.Round(p2.pos.y - p1.pos.y, MidpointRounding.AwayFromZero));
        //    int stepx = 1;
        //    int stepy = 1;

        //    if (dx >= 0)
        //    {
        //        stepx = 1;
        //    }
        //    else
        //    {
        //        stepx = -1;
        //        dx = System.Math.Abs(dx);
        //    }

        //    if (dy >= 0)
        //    {
        //        stepy = 1;
        //    }
        //    else
        //    {
        //        stepy = -1;
        //        dy = System.Math.Abs(dy);
        //    }

        //    int dx2 = 2 * dx;
        //    int dy2 = 2 * dy;

        //    if (dx > dy)
        //    {
        //        int error = dy2 - dx;
        //        for (int i = 0; i <= dx; i++)
        //        {
        //            _canvasBuff.SetPixel(x, y, System.Drawing.Color.White);
        //            if (error >= 0)
        //            {
        //                error -= dx2;
        //                y += stepy;
        //            }
        //            error += dy2;
        //            x += stepx;

        //        }
        //    }
        //    else
        //    {
        //        int error = dx2 - dy;
        //        for (int i = 0; i <= dy; i++)
        //        {
        //            _canvasBuff.SetPixel(x, y, System.Drawing.Color.White);
        //            if (error >= 0)
        //            {
        //                error -= dy2;
        //                x += stepx;
        //            }
        //            error += dx2;
        //            y += stepy;

        //        }
        //    }

        //}


        private void TriangleRasterization(Vertex p1, Vertex p2, Vertex p3, Material material)
        {
            if (p1.pos.y == p2.pos.y)
            {
                if (p1.pos.y < p3.pos.y)
                {//平顶
                    DrawTriangleTop(p1, p2, p3, material);
                }
                else
                {//平底
                    DrawTriangleBottom(p3, p1, p2, material);
                }
            }
            else if (p1.pos.y == p3.pos.y)
            {
                if (p1.pos.y < p2.pos.y)
                {//平顶
                    DrawTriangleTop(p1, p3, p2, material);
                }
                else
                {//平底
                    DrawTriangleBottom(p2, p1, p3, material);
                }
            }
            else if (p2.pos.y == p3.pos.y)
            {
                if (p2.pos.y < p1.pos.y)
                {//平顶
                    DrawTriangleTop(p2, p3, p1, material);
                }
                else
                {//平底
                    DrawTriangleBottom(p1, p2, p3, material);
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

                // 插值生成
                ScreenSpaceLerpVertex(ref newMiddle, top, bottom, t);

                //平顶
                DrawTriangleTop(newMiddle, middle, bottom, material);
                //平底
                DrawTriangleBottom(top, newMiddle, middle, material);
                
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

            v.pos.z = 1 / onePreZ_Middle;

            Vector2 uvPreZ_Top = new Vector2(v1.uv.x * onePreZ_Top, v1.uv.y * onePreZ_Top);
            Vector2 uvPreZ_Bottom = new Vector2(v2.uv.x * onePreZ_Bottom, v2.uv.y * onePreZ_Bottom);
            Vector2 uvPreZ_Middle = MathUtil.Lerp(uvPreZ_Top, uvPreZ_Bottom, t);
            Vector2 uv = uvPreZ_Middle / onePreZ_Middle;

            v.uv = uv;
        }


        private void DrawTriangleTop(Vertex p1, Vertex p2, Vertex p3, Material material)
        {
            for (float y = p1.pos.y; y <= p3.pos.y; y += 0.5f)
            {
                int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
                if (yIndex >= 0 && yIndex < this._screenHeight)
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
                        ScanlineFill(new1, new2, yIndex, material);
                    }
                    else
                    {
                        ScanlineFill(new2, new1, yIndex, material);
                    }
                }
            }
        }

        private void DrawTriangleBottom(Vertex p1, Vertex p2, Vertex p3, Material material)
        {
            for (float y = p1.pos.y; y <= p2.pos.y; y += 0.5f)
            {
                int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
                if (yIndex >= 0 && yIndex < this._screenHeight)
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
                        ScanlineFill(new1, new2, yIndex, material);
                    }
                    else
                    {
                        ScanlineFill(new2, new1, yIndex, material);
                    }
                }
            }
        }

        private void ScanlineFill(Vertex left, Vertex right, int yIndex, Material material)
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
                if (xIndex >= 0 && xIndex < this._screenWidth)
                {
                    float lerpFactor = 0;
                    if (dx != 0)
                    {
                        // 这个也是pos映射到屏幕上的pos',的x',y'的线性插值
                        lerpFactor = (x - left.pos.x) / dx;
                    }
                    //1/z’与x’和y'是线性关系的
                    float onePreZ_Left = 1 / left.pos.z;
                    float onePreZ_Right = 1 / right.pos.z;
                    float onePreZ = MathUtil.Lerp(onePreZ_Left, onePreZ_Right, lerpFactor);

                    //使用1/z进行深度测试
                    //通过测试
                    if (onePreZ >= _zBuff[yIndex, xIndex])
                    {
                        
                        _zBuff[yIndex, xIndex] = onePreZ;
                        Bitmap mainTexture = material.GetMainTexture();
                        //uv 插值，求纹理颜色，s/z 与x' 成正比
                        float u = MathUtil.Lerp(left.uv.x / left.pos.z, right.uv.x / right.pos.z, lerpFactor) / onePreZ * (mainTexture.Width - 1);
                        float v = MathUtil.Lerp(left.uv.y / left.pos.z, right.uv.y / right.pos.z, lerpFactor) / onePreZ * (mainTexture.Height - 1);

                        //纹理采样
                        Color texColor = new Color(1, 1, 1, 1);
                        
                        //点采样
                        if (_textureFilterMode == TextureFilterMode.point)
                        {
                            int uIndex = (int)System.Math.Round(u, MidpointRounding.AwayFromZero);
                            int vIndex = (int)System.Math.Round(v, MidpointRounding.AwayFromZero);
                            uIndex = MathUtil.Clamp(uIndex, 0, mainTexture.Width - 1);
                            vIndex = MathUtil.Clamp(vIndex, 0, mainTexture.Height - 1);
                            //转到我们自定义的color进行计算
                            System.Drawing.Color originCol = ReadTexture(uIndex, vIndex, mainTexture);
                            texColor = new Color(originCol);
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
                            Color texcolor1 = new Color(ReadTexture((int)uIndex, (int)vIndex, mainTexture)) * (1 - du) * (1 - dv);
                            Color texcolor2 = new Color(ReadTexture((int)uIndex + 1, (int)vIndex, mainTexture)) * du * (1 - dv);
                            Color texcolor3 = new Color(ReadTexture((int)uIndex, (int)vIndex + 1, mainTexture)) * (1 - du) * dv;
                            Color texcolor4 = new Color(ReadTexture((int)uIndex + 1, (int)vIndex + 1, mainTexture)) * du * dv;
                            texColor = texcolor1 + texcolor2 + texcolor3 + texcolor4;
                        }

                        if (_renderMode == RenderMode.Textured)
                        {
                            _canvasBuff.SetPixel(xIndex, yIndex, texColor.TransFormToSystemColor());
                        }

                        //插值顶点颜色
                        //SoftRenderer.RenderData.Color vertColor = MathUntil.Lerp(left.vcolor, right.vcolor, lerpFactor) * w;
                        //插值光照颜色
                        //SoftRenderer.RenderData.Color lightColor = MathUntil.Lerp(left.lightingColor, right.lightingColor, lerpFactor) * w; ;


                        //if (_lightMode == LightMode.On)
                        //{//光照模式，需要混合光照的颜色
                        //    if (RenderMode.Textured == _currentMode)
                        //    {
                        //        SoftRenderer.RenderData.Color finalColor = texColor * lightColor;
                        //        _frameBuff.SetPixel(xIndex, yIndex, finalColor.TransFormToSystemColor());
                        //    }
                        //    else if (RenderMode.VertexColor == _currentMode)
                        //    {
                        //        SoftRenderer.RenderData.Color finalColor = vertColor * lightColor;
                        //        _frameBuff.SetPixel(xIndex, yIndex, finalColor.TransFormToSystemColor());
                        //    }
                        //}
                        //else
                        //{
                        //    if (RenderMode.Textured == _currentMode)
                        //    {
                        //        _frameBuff.SetPixel(xIndex, yIndex, texColor.TransFormToSystemColor());
                        //    }
                        //    else if (RenderMode.VertexColor == _currentMode)
                        //    {
                        //        _frameBuff.SetPixel(xIndex, yIndex, vertColor.TransFormToSystemColor());
                        //    }
                        //}
                    }
                    
                }
            }

        }

        private System.Drawing.Color ReadTexture(int uIndex, int vIndex, Bitmap tex)
        {
            int u = MathUtil.Clamp(uIndex, 0, tex.Width - 1);
            int v = MathUtil.Clamp(vIndex, 0, tex.Height - 1);
            return tex.GetPixel(u, v);
        }
    }
}
