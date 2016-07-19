using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;
namespace SoftRenderer.Renderer
{
    class Model : Node
    {
        private Mesh _mesh;
        private Material _material;

        public Model()
        {
            _mesh = null;
            _material = new Material();
        }

        public void SetMesh(Vertex[] v, int[] indexs)
        {
            _mesh = new Mesh(v, indexs);
        }

        public void SetMesh(Vector4[] posList, Vector2[] uvList, int[] indexs)
        {
            List<Vertex> vertexList = new List<Vertex>();
            for (int i = 0; i < posList.Length; i++)
            {
                Vertex v = new Vertex();
                v.pos = posList[i];
                v.uv = uvList[i];
                vertexList.Add(v);
            }
            SetMesh(vertexList.ToArray(), indexs);
        }

        public Mesh GetMesh()
        {
            return _mesh;
        }

        public void SetMaterial(Material mat)
        {
            _material = mat;
        }

        public Material GetMaterial()
        {
            return _material;
        }
    }
}
