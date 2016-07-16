using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;
namespace SoftRenderer.Renderer
{
    // Model, Particle, ...的基类
    class Node
    {
        // Transform
        public Vector3 pos;
        public Quaternion rotation;
        public Vector3 scale;

        private Matrix4x4 _worldMatrix;

        public Node()
        {
            _worldMatrix = new Matrix4x4();
        }
        public Matrix4x4 GetWorldMatrix()
        {
            // only pos
            _worldMatrix.CreateTranslation(pos.x, pos.y, pos.z);
            return _worldMatrix;
        }
    }
}
