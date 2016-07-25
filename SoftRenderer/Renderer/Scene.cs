using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;
namespace SoftRenderer.Renderer
{
    class Scene
    {
        private List<Node> _nodeList = new List<Node>();
        private Camera _activeCamera = null;
        public void AddNode(Node n)
        {
            _nodeList.Add(n);
        }

        public void SetActiveCamera(Camera cam)
        {
            _activeCamera = cam;
        }

        public void Draw()
        {
            if (_activeCamera != null)
            {
                // 获得Camera的矩阵，vp
                Matrix4x4 viewMat = _activeCamera.GetViewMatrix();
                Matrix4x4 projMat = _activeCamera.GetProjectionMatrix();
                for (int i = 0; i < _nodeList.Count; i++)
                {
                    Node node = _nodeList[i];
                    Matrix4x4 worldMat = node.GetWorldMatrix();

                    if (node is Model)
                    {
                        Model model = node as Model;
                        Mesh mesh = model.GetMesh();
                        Material material = model.GetMaterial();
                        Vertex[] vertexList = mesh.GetVertices();
                        //Util.TransitionVertexList(ref vertexList, worldMat, viewMat, projMat);
                        //Rasterizetion.Rasterizer.Instance().Render(Rasterizetion.PrimitiveMode.Lines, vertexList);

                    }
                }
            }
            
        }
    }
}
