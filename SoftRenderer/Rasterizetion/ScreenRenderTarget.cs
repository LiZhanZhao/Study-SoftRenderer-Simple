using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
namespace SoftRenderer.Rasterizetion
{
    public class ScreenRenderTarget : RenderTarget
    {
        private Bitmap _frameBuffer = null;
        private int _screenWidth, _screenHeight;
        public ScreenRenderTarget(Bitmap fb)
        {
            _frameBuffer = fb;
        }

        override public void Write(int x, int y, System.Drawing.Color col)
        {
            _frameBuffer.SetPixel(x, y, col);
        }
        public override int Width()
        {
            return _screenWidth;
        }
        public override int Height()
        {
            return _screenHeight;
        }

        public void SetScreenWidthHeight(int w, int h)
        {
            _screenWidth = w;
            _screenHeight = h;
        }

    }
}
