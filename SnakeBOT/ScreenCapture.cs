using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBOT
{
    public class ScreenCapture
    {
        public Bitmap PrintScreen(Point ALocation, Size ASize)
        {
            Bitmap LBitmap = new Bitmap(ASize.Width, ASize.Height);
            Graphics LGraphics = Graphics.FromImage(LBitmap);

            LGraphics.CopyFromScreen(ALocation, Point.Empty, ASize);

            LGraphics.Dispose();

            return LBitmap;
        }
    }
}
