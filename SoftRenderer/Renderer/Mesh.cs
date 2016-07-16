using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;

namespace SoftRenderer.Renderer
{
    class Mesh
    {
        // 顶点数据
        private List<Vertex> _vertices = new List<Vertex>();
        // 索引
        private List<int> _triangles = new List<int>();
        // 顶点数量
        private int _vectexCount;

        public Mesh(Vertex[] v, int[] indexs)
        {
            _vertices.AddRange(v);
            _triangles.AddRange(indexs);
        }

        public Vertex[] GetVertices()
        {
            return _vertices.ToArray();
        }

        public int[] GetTriangles()
        {
            return _triangles.ToArray();
        }





    }
}
