using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 1  0  0  x
 * 0  1  0  y
 * 0  0  1  z
 * 0  0  0  1
*/
namespace SoftRenderer.Math
{
    class Matrix4x4
    {
        public float[] m = new float[16];

        public Matrix4x4() 
        {
            Identity();
        }

        public void Identity()
        {
            m[0] = 1;
            m[1] = 0;
            m[2] = 0;
            m[3] = 0;

            m[4] = 0;
            m[5] = 1;
            m[6] = 0;
            m[7] = 0;

            m[8] = 0;
            m[9] = 0;
            m[10] = 1;
            m[11] = 0;

            m[12] = 0;
            m[13] = 0;
            m[14] = 0;
            m[15] = 1;

        }

        /**
        * Creates a matrix initialized to the specified column-major array.
        *
        * The passed-in array is in column-major order, so the memory layout of the array is as follows:
        *
        *     0   4   8   12
        *     1   5   9   13
        *     2   6   10  14
        *     3   7   11  15
        *
        * @param m An array containing 16 elements in column-major order.
        */

        public Matrix4x4(float[] m)
        {
            this.m = m;
        }


        /**
        * Constructs a matrix initialized to the specified value.
        *
        * @param m11 The first element of the first row.
        * @param m12 The second element of the first row.
        * @param m13 The third element of the first row.
        * @param m14 The fourth element of the first row.
        * @param m21 The first element of the second row.
        * @param m22 The second element of the second row.
        * @param m23 The third element of the second row.
        * @param m24 The fourth element of the second row.
        * @param m31 The first element of the third row.
        * @param m32 The second element of the third row.
        * @param m33 The third element of the third row.
        * @param m34 The fourth element of the third row.
        * @param m41 The first element of the fourth row.
        * @param m42 The second element of the fourth row.
        * @param m43 The third element of the fourth row.
        * @param m44 The fourth element of the fourth row.
        * 
        *     m11   m12   m13   m14
        *     m21   m22   m23   m24
        *     m31   m32   m33   m34
        *     m41   m42   m43   m44
        */
        public Matrix4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
                float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            m[0] = m11;
            m[1] = m21;
            m[2] = m31;
            m[3] = m41;
            m[4] = m12;
            m[5] = m22;
            m[6] = m32;
            m[7] = m42;
            m[8] = m13;
            m[9] = m23;
            m[10] = m33;
            m[11] = m43;
            m[12] = m14;
            m[13] = m24;
            m[14] = m34;
            m[15] = m44;
        }

        public void CreateTranslation(float x, float y, float z)
        {
            m[12] = x;
            m[13] = y;
            m[14] = z;
        }

        public void CreateLookAt(float eyePositionX, float eyePositionY, float eyePositionZ,
                          float targetPositionX, float targetPositionY, float targetPositionZ,
                          float upX, float upY, float upZ)
        {

            Vector4 eye = new Vector4(eyePositionX, eyePositionY, eyePositionZ, 1);
            Vector4 target = new Vector4(targetPositionX, targetPositionY, targetPositionZ, 1);
            Vector4 up = new Vector4(upX, upY, upZ, 0);
            up.Normalize();
            // 如果是zaxis = targetPos - eye的话，那么就是eye 指向 targetPos 的方向是Camera的forward方向，
            // 但是，Camera的Back方向才是真正看物体的方向。(OpenGl), 那就是，targetPos 执行eye 的方向才是看物体的方向。
            //Vector4 zaxis = target - eye;

            // 这个是与上面相反，那么，看物体的方向就是，eye 指向 targetPos 的方向。(GamePlay的做法)
            Vector4 zaxis = eye - target;
            zaxis.Normalize();

            Vector4 xaxis = Vector4.Cross(up, zaxis);
            xaxis.Normalize();

            Vector4 yaxis = Vector4.Cross(zaxis, xaxis);
            yaxis.Normalize();

            m[0] = xaxis.x;
            m[1] = yaxis.x;
            m[2] = zaxis.x;
            m[3] = 0.0f;

            m[4] = xaxis.y;
            m[5] = yaxis.y;
            m[6] = zaxis.y;
            m[7] = 0.0f;

            m[8] = xaxis.z;
            m[9] = yaxis.z;
            m[10] = zaxis.z;
            m[11] = 0.0f;

            m[12] = -Vector4.Dot(xaxis, eye);
            m[13] = -Vector4.Dot(yaxis, eye);
            m[14] = -Vector4.Dot(zaxis, eye);
            m[15] = 1.0f;
        }


        public void CreatePerspective(float fieldOfView, float aspectRatio,
                                     float zNearPlane, float zFarPlane)
        {
            this.SetZero();
            float f_n = 1.0f / (zFarPlane - zNearPlane);
            // 当使用Math类的三角函数的时候，所有的单位都是用弧度表示的
            float theta = MathUtil.ConvertDegreesToRadians(fieldOfView) * 0.5f;
            float divisor = (float)System.Math.Tan(theta);
            float factor = 1.0f / divisor;
            m[0] = (1.0f / aspectRatio) * factor;
            m[5] = factor;
            m[10] = (-(zFarPlane + zNearPlane)) * f_n;
            m[11] = -1.0f;
            m[14] = -2.0f * zFarPlane * zNearPlane * f_n;
        }

        public static Vector4 operator *(Matrix4x4 matrix, Vector4 v)
        {
            Vector4 res = new Vector4();
            res.x = v.x * matrix.m[0] + v.y * matrix.m[4] + v.z * matrix.m[8] + v.w * matrix.m[12];
            res.y = v.x * matrix.m[1] + v.y * matrix.m[5] + v.z * matrix.m[9] + v.w * matrix.m[13];
            res.z = v.x * matrix.m[2] + v.y * matrix.m[6] + v.z * matrix.m[10] + v.w * matrix.m[14];
            res.w = v.x * matrix.m[3] + v.y * matrix.m[7] + v.z * matrix.m[11] + v.w * matrix.m[15];
            
            return res;
        }

        public void SetZero()
        {
            for (int i = 0; i < 16; i++)
            {
                m[i] = 0;
            }
        }


        public void CreateRotationY(float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);

            m[0]  = c;
            m[2]  = -s;
            m[8]  = s;
            m[10] = c;
        }
    }
}
