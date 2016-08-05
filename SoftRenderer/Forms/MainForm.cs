using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

using SoftRenderer.Renderer;
using SoftRenderer.Math;
using SoftRenderer.Rasterizetion;
namespace SoftRenderer.Forms
{
    class MainForm : Form
    {
        // 画布
        private Bitmap _canvasBuff = null;
        // clear and draw 这个画布,是关于画布,和屏幕没有关系
        private Graphics _canvasGrapClear = null;
        private Graphics _canvasGrapDraw = null;
        // 这个才是与屏幕有关的
        private Graphics _screenGrapDraw = null;
        private float[,] _zBuff;

        private Image _texture = null;
        private int _count = 0;
        private int runCount = 0;
        private Scene _scene;

        public MainForm()
        {
            InitForm();
            InitCanvasAndGraphics();
            //InitZBuff();
            //InitRenderer();



            Application.Idle += HandleApplicationIdle;
            Init();
            
            
        }

        private void InitForm()
        {
            this.Text = "SfRenderer";
            this.BackColor = System.Drawing.Color.Black;
            this.MaximumSize = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            
            
        }

        private void InitCanvasAndGraphics()
        {
            _canvasBuff = new Bitmap(this.MaximumSize.Width, this.MaximumSize.Height);
            _canvasGrapClear = Graphics.FromImage(_canvasBuff);
            _canvasGrapDraw = Graphics.FromImage(_canvasBuff);
            _screenGrapDraw = CreateGraphics();
        }

        private void InitZBuff()
        {
            _zBuff = new float[this.MaximumSize.Height, this.MaximumSize.Width];
        }

        private void Clear()
        {
            //Array.Clear(_zBuff, 0, _zBuff.Length);
            _canvasGrapClear.Clear(System.Drawing.Color.Black);
        }
        //private void InitRenderer()
        //{
        //    Renderer.Renderer.Instance().SetRenderMode(RenderMode.Wireframe);
        //    Renderer.Renderer.Instance().SetRenderMode(RenderMode.Textured);
        //    Renderer.Renderer.Instance().SetCanvasBuff(_canvasBuff);
        //    Renderer.Renderer.Instance().SetScreenWidthHeight(this.MaximumSize.Width, this.MaximumSize.Height);
        //    Renderer.Renderer.Instance().SetZBuff(_zBuff);

            
        //}

        void HandleApplicationIdle(object sender, EventArgs e)
        {
            
            while (IsApplicationIdle())
            {
                Update();
                Render();
            }
        }

        // 如果Application Idle的话，就返回True
        bool IsApplicationIdle()
        {
            NativeMessage result;
            return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        private Model _model;
        void Init()
        {
            //Util.screenWidth = this.MaximumSize.Width;
            //Util.screenHeight = this.MaximumSize.Height;
            ScreenRenderTarget target = new ScreenRenderTarget(_canvasBuff);
            target.SetScreenWidthHeight(this.MaximumSize.Width, this.MaximumSize.Height);
            Rasterizer.Instance().SetRenderTarget(target);
            

            //_scene = new Scene();
            //_model = new Model();
            //_model.SetMesh(TestData.pointList, TestData.uvs, TestData.indexs);
            //_model.pos = new Math.Vector3(0, 0, 10);

            //Material mat = new Material("../../Texture/texture.jpg");
            //_model.SetMaterial(mat);

            //_scene.AddNode(_model);

            // 改用角度，不用弧度
            //Camera cam = new Camera(new Vector4(5, 10, -10, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 1, 0, 0), 45, this.MaximumSize.Width / (float)this.MaximumSize.Height, 1f, 500f);
            //_scene.SetActiveCamera(cam);

            
        }

        void Update()
        {
            /*
            runCount++;
            if (runCount % 15 == 0)
            {
                _count++;
                _count = _count % 10;
                _model.pos = new Math.Vector3(0, 0, _count);
                runCount = 0;
            }
             * */
            
        }

        void Render()
        {
            Clear();
            //_scene.Draw();

            // 画线
            //Rasterizetion.Rasterizer.Instance().Render(Rasterizetion.PrimitiveMode.Lines, TestData.lineVertexList);

            // 画三角形
            Rasterizetion.Rasterizer.Instance().Render(Rasterizetion.PrimitiveMode.Triangles, TestData.triVertexList);

            _screenGrapDraw.DrawImage(_canvasBuff, 0, 0);
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);


        

       
    }
}
