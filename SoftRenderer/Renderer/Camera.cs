using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoftRenderer.Math;
namespace SoftRenderer.Renderer
{
    class Camera
    {
        public Vector4 pos;
        public Vector4 lookAt;
        public Vector4 up;
        public float fov;
        public float aspect;
        public float zn;
        public float zf;

        private Matrix4x4 _viewMatrix;
        private Matrix4x4 _projectionMatrix;

        public Matrix4x4 GetViewMatrix()
        {
            _viewMatrix.CreateLookAt(pos.x, pos.y, pos.z, lookAt.x, lookAt.y, lookAt.z,
                up.x, up.y, up.z);
            return _viewMatrix;
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            _projectionMatrix.CreatePerspective(fov, aspect, zn, zf);
            return _projectionMatrix;
        }


        public Camera(Vector4 pos, Vector4 lookAt, Vector4 up, float fov, float aspect, float zn, float zf)
        {
            this.pos = pos;
            this.lookAt = lookAt;
            this.up = up;
            this.fov = fov;
            this.aspect = aspect;
            this.zn = zn;
            this.zf = zf;

            _viewMatrix = new Matrix4x4();
            _projectionMatrix = new Matrix4x4();
        }
    }
}
