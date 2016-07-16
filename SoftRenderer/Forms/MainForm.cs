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

        private Image _texture = null;
        private int _count = 0;

        private Scene _scene;

        public MainForm()
        {
            InitForm();
            InitCanvasAndGraphics();
            InitRenderer();
            Application.Idle += HandleApplicationIdle;
            Init();
            
            
        }

        private void InitForm()
        {
            this.Text = "SfRenderer";
            this.BackColor = Color.Black;
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

        private void InitRenderer()
        {
            Renderer.Renderer.Instance().SetRenderMode(RenderMode.Wireframe);
            Renderer.Renderer.Instance().SetCanvasBuff(_canvasBuff);
            Renderer.Renderer.Instance().SetScreenWidthHeight(this.MaximumSize.Width, this.MaximumSize.Height);
        }

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

        void Init()
        {

            //Console.WriteLine("11");
            //_texture = System.Drawing.Image.FromFile("../../Texture/texture.jpg");   
            //建立一个scene,add node 
            _scene = new Scene();

            Model m = new Model();
            m.SetMesh(TestData.pointList, TestData.indexs);
            m.pos = new Math.Vector3(0, 0, -10);

            _scene.AddNode(m);

            Camera cam = new Camera(new Vector4(0, 0, 0, 1), new Vector4(0, 0, -1, 1), new Vector4(0, 1, 0, 0), (float)System.Math.PI / 4, this.MaximumSize.Width / (float)this.MaximumSize.Height, 1f, 500f);
            _scene.SetActiveCamera(cam);
        }

        void Update()
        {
            
            
        }

        void Render()
        {

            _canvasGrapClear.Clear(Color.Black);

            
            

            //_count++;
            //_count = _count % this.MaximumSize.Width;
            //// 往canvas画东西
            //_canvasGrapDraw.DrawImage(_texture, _count, 0);
            _scene.Draw();

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
