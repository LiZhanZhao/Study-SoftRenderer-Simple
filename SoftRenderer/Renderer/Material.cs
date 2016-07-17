using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
namespace SoftRenderer.Renderer
{
    class Material
    {
        private Bitmap _mainTexture = null;
        public Material()
        {
            _mainTexture = new Bitmap(256, 256);
            InitTexture(_mainTexture);
        }

        public Material(string path)
        {
            this.SetMainTexture(path);
        }

        public void SetMainTexture(string path)
        {
            Image img = Image.FromFile(path);
            _mainTexture = new Bitmap(img, img.Width, img.Height);
        }

        public Bitmap GetMainTexture()
        {
            return _mainTexture;
        }

        void InitTexture(Bitmap tex)
        {
            for (int j = 0; j < 256; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    tex.SetPixel(j, i, ((j + i) % 32 == 0) ? System.Drawing.Color.White : System.Drawing.Color.Green);
                }
            }
        }
    }
}
