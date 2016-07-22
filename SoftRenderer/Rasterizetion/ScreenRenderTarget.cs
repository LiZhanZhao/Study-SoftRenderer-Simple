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
        public ScreenRenderTarget(Bitmap fb)
        {
            _frameBuffer = fb;
        }

        override public void Write(int x, int y, System.Drawing.Color col)
        {
            _frameBuffer.SetPixel(x, y, col);
        }
    }
}
