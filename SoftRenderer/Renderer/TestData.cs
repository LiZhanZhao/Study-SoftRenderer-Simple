using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;

namespace SoftRenderer.Renderer
{
    class TestData
    {
        // 顶点数据
        public static Vertex[] lineVertexList = {
                                                new Vertex(new Vector4(0,  0, 0, 1), new Color(1, 0, 0, 1)),
                                                new Vertex(new Vector4(100, 100, 0, 1), new Color(1, 1, 1, 1)),
                                                new Vertex(new Vector4(100, 100, 0, 1), new Color(1, 1, 1, 1)),
                                                new Vertex(new Vector4(200, 100, 1, 1), new Color(0, 0, 1, 1)),
                                            };
        
        public static Vertex[] triVertexList = {
                                                   //new Vertex(new Vector4(-1,  -1f, 0, 1), new Color(1, 0, 0, 1)),
                                                   //new Vertex(new Vector4(-1,  1f, 0, 1), new Color(0, 1, 0, 1)),
                                                   //new Vertex(new Vector4(1,  -1f, 0, 1), new Color(0, 0, 1, 1)),

                                                   new Vertex(new Vector4(0,  0.5f, 0, 1), new Color(1, 0, 0, 1)),
                                                   new Vertex(new Vector4(0.5f,  0.5f, 0, 1), new Color(0, 0, 1, 1)),
                                                   new Vertex(new Vector4(0f,  0f, -2, 1), new Color(0, 1, 0, 1)),
                                                   
                                               };

        //顶点坐标
        public static Vector4[] pointList = {
                                            new Vector4(-1,  1, -1, 1),
                                            new Vector4(-1, -1, -1, 1),
                                            new Vector4(1, -1, -1, 1),
                                            new Vector4(1, 1, -1, 1),

                                            new Vector4( -1,  1, 1, 1),
                                            new Vector4(-1, -1, 1, 1),
                                            new Vector4(1, -1, 1, 1),
                                            new Vector4(1, 1, 1, 1)
                                        };
        public static Vector2[] uvs ={
                                  new Vector2(0, 0),new Vector2( 0, 1),new Vector2(1, 1),
                                   new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
                                   //
                                    new Vector2(0, 0),new Vector2( 0, 1),new Vector2(1, 1),
                                   new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
                                   //
                                    new Vector2(0, 0),new Vector2( 0, 1),new Vector2(1, 1),
                                   new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
                                   //
                                    new Vector2(0, 0),new Vector2( 0, 1),new Vector2(1, 1),
                                   new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
                                   //
                                     new Vector2(0, 0),new Vector2( 0, 1),new Vector2(1, 1),
                                   new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
                                   ///
                                     new Vector2(0, 0),new Vector2( 0, 1),new Vector2(1, 1),
                                   new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0)
                              };

        //三角形顶点索引 12个面
        public static int[] indexs = {
                                    0,1,2,
                                   0,2,3,
                                   //
                                   7,6,5,
                                   7,5,4,
                                   //
                                   0,4,5,
                                   0,5,1,
                                   //
                                   1,5,6,
                                   1,6,2,
                                   //
                                   2,6,7,
                                   2,7,3,
                                   //
                                   3,7,4,
                                   3,4,0
                               };
    }
}
